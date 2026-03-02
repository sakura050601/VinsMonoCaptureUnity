using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VinsMonoCapture.DataModels;
using VinsMonoCapture.Export;
using VinsMonoCapture.Native;

namespace VinsMonoCapture.Capture
{
    public class CaptureSessionController : MonoBehaviour
    {
        [SerializeField] private string sessionName = "default_session";
        [SerializeField] private bool useEditorMockBridge = false;
        [SerializeField] private double imuUpdateIntervalSeconds = 0.005;

        private ICameraCaptureBridge cameraBridge = null!;
        private IImuCaptureBridge imuBridge = null!;

        private readonly List<ImageFrameRecord> frameRecords = new();
        private readonly List<ImuSampleRecord> accelerometerSamples = new();
        private readonly List<ImuSampleRecord> gyroscopeSamples = new();
        private readonly List<string> inMemoryLogs = new();

        private SessionFolderManager sessionFolderManager = null!;
        private FileExportService fileExportService = null!;
        private SessionPaths currentSessionPaths = null!;
        private SessionMetadataModel metadata = new();

        private bool isCapturing;
        private string latestIntrinsicsJson = string.Empty;

        public event Action<int, int, int, string>? StatusUpdated;

        public void SetSessionName(string newSessionName) => sessionName = newSessionName;

        private void Awake()
        {
            sessionFolderManager = new SessionFolderManager();
            fileExportService = new FileExportService(new CsvWriterService(), new JsonWriterService());

#if UNITY_EDITOR
            if (useEditorMockBridge)
            {
                var mockBridge = gameObject.AddComponent<EditorMockCaptureBridge>();
                cameraBridge = mockBridge;
                imuBridge = mockBridge;
            }
            else
            {
                var iosBridge = new IosCaptureBridge();
                cameraBridge = iosBridge;
                imuBridge = iosBridge;
            }
#else
            var iosBridge = new IosCaptureBridge();
            cameraBridge = iosBridge;
            imuBridge = iosBridge;
#endif

            cameraBridge.FrameReceived += OnFrameReceived;
            cameraBridge.IntrinsicsReceived += OnIntrinsicsReceived;
            imuBridge.AccelerometerSampleReceived += OnAccelerometerReceived;
            imuBridge.GyroscopeSampleReceived += OnGyroscopeReceived;
        }

        public void StartCapture()
        {
            if (isCapturing) return;

            frameRecords.Clear();
            accelerometerSamples.Clear();
            gyroscopeSamples.Clear();
            inMemoryLogs.Clear();

            currentSessionPaths = sessionFolderManager.CreateSessionFolders(Application.persistentDataPath, sessionName);
            metadata = new SessionMetadataModel
            {
                SessionName = sessionName,
                SessionIdentifier = currentSessionPaths.SessionIdentifier,
                CaptureStartIso8601 = DateTime.UtcNow.ToString("O"),
                DeviceModel = SystemInfo.deviceModel,
                OperatingSystem = SystemInfo.operatingSystem
            };

            Log($"Capture started: {currentSessionPaths.SessionDirectoryPath}");
            cameraBridge.StartCameraCapture();
            imuBridge.StartImuCapture(imuUpdateIntervalSeconds);
            isCapturing = true;
            EmitStatus("采集中");
        }

        public void StopCapture()
        {
            if (!isCapturing) return;

            cameraBridge.StopCameraCapture();
            imuBridge.StopImuCapture();
            isCapturing = false;

            metadata.CapturedFrameCount = frameRecords.Count;
            metadata.CapturedAccelerometerSampleCount = accelerometerSamples.Count;
            metadata.CapturedGyroscopeSampleCount = gyroscopeSamples.Count;
            metadata.CaptureEndIso8601 = DateTime.UtcNow.ToString("O");

            var intrinsicsPath = Path.Combine(currentSessionPaths.SessionDirectoryPath, "intrinsics_from_callback.json");
            if (!string.IsNullOrWhiteSpace(latestIntrinsicsJson))
            {
                File.WriteAllText(intrinsicsPath, latestIntrinsicsJson);
            }

            fileExportService.ExportSession(currentSessionPaths, metadata, frameRecords, accelerometerSamples, gyroscopeSamples, intrinsicsPath, string.Join("\n", inMemoryLogs));
            Log("Capture stopped and session exported");
            EmitStatus("已停止");
        }

        private void OnFrameReceived(CameraFramePayload payload)
        {
            var fileName = $"frame_{payload.FrameIndex:D6}.jpg";
            var filePath = Path.Combine(currentSessionPaths.ImageDirectoryPath, fileName);
            File.WriteAllBytes(filePath, payload.EncodedImageBytes);

            frameRecords.Add(new ImageFrameRecord
            {
                FrameIndex = payload.FrameIndex,
                FileName = fileName,
                TimestampSeconds = payload.TimestampSeconds,
                ImageWidth = payload.Width,
                ImageHeight = payload.Height,
                PixelFormat = payload.PixelFormat
            });

            EmitStatus("采集中");
        }

        private void OnIntrinsicsReceived(string intrinsicsJson)
        {
            latestIntrinsicsJson = intrinsicsJson;
            Log("Camera intrinsics callback received");
        }

        private void OnAccelerometerReceived(ImuPayload payload)
        {
            accelerometerSamples.Add(new ImuSampleRecord
            {
                TimestampSeconds = payload.TimestampSeconds,
                X = payload.X,
                Y = payload.Y,
                Z = payload.Z,
                Unit = "m/s^2"
            });
            EmitStatus("采集中");
        }

        private void OnGyroscopeReceived(ImuPayload payload)
        {
            gyroscopeSamples.Add(new ImuSampleRecord
            {
                TimestampSeconds = payload.TimestampSeconds,
                X = payload.X,
                Y = payload.Y,
                Z = payload.Z,
                Unit = "rad/s"
            });
            EmitStatus("采集中");
        }

        private void EmitStatus(string message)
        {
            StatusUpdated?.Invoke(frameRecords.Count, accelerometerSamples.Count, gyroscopeSamples.Count, message);
        }

        private void Log(string message)
        {
            var line = $"[{DateTime.UtcNow:O}] {message}";
            Debug.Log(line);
            inMemoryLogs.Add(line);
        }
    }
}
