using System;

namespace VinsMonoCapture.DataModels
{
    [Serializable]
    public class ImuSampleRecord
    {
        public double TimestampSeconds;
        public double X;
        public double Y;
        public double Z;
        public string Unit = string.Empty;
        public string CoordinateFrame = "device";
    }
}
