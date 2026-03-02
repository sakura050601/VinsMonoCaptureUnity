using System;

namespace VinsMonoCapture.DataModels
{
    [Serializable]
    public class SessionMetadataModel
    {
        public string SessionName = string.Empty;
        public string SessionIdentifier = string.Empty;
        public string CaptureStartIso8601 = string.Empty;
        public string CaptureEndIso8601 = string.Empty;

        public string TimestampSourceCamera = "AVFoundation CMSampleBuffer presentation timestamp";
        public string TimestampSourceImu = "CoreMotion deviceMotion timestamp bridged to monotonic clock domain";

        public string DeviceModel = string.Empty;
        public string OperatingSystem = string.Empty;
        public string AppVersion = "0.1.0";

        public int CapturedFrameCount;
        public int CapturedAccelerometerSampleCount;
        public int CapturedGyroscopeSampleCount;

        public CameraIntrinsicsModel CameraIntrinsics = new();
        public string Notes = string.Empty;
    }

    [Serializable]
    public class CameraIntrinsicsModel
    {
        public double FocalLengthX;
        public double FocalLengthY;
        public double PrincipalPointX;
        public double PrincipalPointY;
        public int ImageWidth;
        public int ImageHeight;
        public string DistortionModel = "unknown";
        public bool IntrinsicsAvailable;
        public string Source = "AVFoundation";
    }
}
