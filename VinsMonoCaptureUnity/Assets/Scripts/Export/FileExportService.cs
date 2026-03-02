using System;
using System.Collections.Generic;
using System.IO;
using VinsMonoCapture.DataModels;

namespace VinsMonoCapture.Export
{
    public class FileExportService
    {
        private readonly CsvWriterService csvWriterService;
        private readonly JsonWriterService jsonWriterService;

        public FileExportService(CsvWriterService csvWriterService, JsonWriterService jsonWriterService)
        {
            this.csvWriterService = csvWriterService;
            this.jsonWriterService = jsonWriterService;
        }

        public void ExportSession(SessionPaths sessionPaths, SessionMetadataModel metadata, IReadOnlyList<ImageFrameRecord> frames,
            IReadOnlyList<ImuSampleRecord> accelerometerSamples, IReadOnlyList<ImuSampleRecord> gyroscopeSamples,
            string cameraIntrinsicsPath, string logContent)
        {
            csvWriterService.WriteFrameTimestampCsv(Path.Combine(sessionPaths.SessionDirectoryPath, "frame_index_to_timestamp.csv"), frames);
            csvWriterService.WriteImuCsv(Path.Combine(sessionPaths.SessionDirectoryPath, "imu_accel.csv"), accelerometerSamples);
            csvWriterService.WriteImuCsv(Path.Combine(sessionPaths.SessionDirectoryPath, "imu_gyro.csv"), gyroscopeSamples);

            jsonWriterService.WriteJson(Path.Combine(sessionPaths.SessionDirectoryPath, "session_metadata.json"), metadata);

            if (File.Exists(cameraIntrinsicsPath))
            {
                File.Copy(cameraIntrinsicsPath, Path.Combine(sessionPaths.SessionDirectoryPath, "camera_intrinsics.json"), true);
            }
            else
            {
                jsonWriterService.WriteJson(Path.Combine(sessionPaths.SessionDirectoryPath, "camera_intrinsics.json"), metadata.CameraIntrinsics);
            }

            File.WriteAllText(Path.Combine(sessionPaths.LogsDirectoryPath, "logs.txt"), logContent + Environment.NewLine);
        }
    }
}
