using System;

namespace VinsMonoCapture.DataModels
{
    [Serializable]
    public class ImageFrameRecord
    {
        public long FrameIndex;
        public string FileName = string.Empty;
        public double TimestampSeconds;
        public int ImageWidth;
        public int ImageHeight;
        public string PixelFormat = "jpeg";
    }
}
