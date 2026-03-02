using System;
using System.Text;
using UnityEngine;

namespace VinsMonoCapture.Native
{
    public class EditorMockCaptureBridge : MonoBehaviour, ICameraCaptureBridge, IImuCaptureBridge
    {
        public event Action<CameraFramePayload> FrameReceived;
        public event Action<string> IntrinsicsReceived;
        public event Action<ImuPayload> AccelerometerSampleReceived;
        public event Action<ImuPayload> GyroscopeSampleReceived;

        private bool isCameraRunning;
        private bool isImuRunning;
        private long frameIndex;
        private float imuTime;

        public void StartCameraCapture()
        {
            isCameraRunning = true;
            frameIndex = 0;
            IntrinsicsReceived?.Invoke("{\"FocalLengthX\":900.0,\"FocalLengthY\":900.0,\"PrincipalPointX\":640.0,\"PrincipalPointY\":360.0,\"ImageWidth\":1280,\"ImageHeight\":720,\"IntrinsicsAvailable\":true}");
        }

        public void StopCameraCapture() => isCameraRunning = false;

        public void StartImuCapture(double updateIntervalSeconds)
        {
            isImuRunning = true;
            imuTime = 0f;
        }

        public void StopImuCapture() => isImuRunning = false;

        private void Update()
        {
            if (isCameraRunning && Time.frameCount % 3 == 0)
            {
                var dummyBytes = Encoding.ASCII.GetBytes($"mock_frame_{frameIndex}");
                FrameReceived?.Invoke(new CameraFramePayload
                {
                    FrameIndex = frameIndex,
                    TimestampSeconds = Time.realtimeSinceStartupAsDouble,
                    EncodedImageBytes = dummyBytes,
                    Width = 1280,
                    Height = 720,
                    PixelFormat = "jpeg"
                });
                frameIndex++;
            }

            if (isImuRunning)
            {
                imuTime += Time.deltaTime;
                var timestamp = Time.realtimeSinceStartupAsDouble;
                AccelerometerSampleReceived?.Invoke(new ImuPayload { TimestampSeconds = timestamp, X = Math.Sin(imuTime), Y = 0.1, Z = 9.81 });
                GyroscopeSampleReceived?.Invoke(new ImuPayload { TimestampSeconds = timestamp, X = 0.01, Y = Math.Cos(imuTime), Z = 0.02 });
            }
        }
    }
}
