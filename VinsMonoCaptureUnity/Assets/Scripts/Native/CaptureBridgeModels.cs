namespace VinsMonoCapture.Native
{
    public struct CameraFramePayload
    {
        public long FrameIndex;
        public double TimestampSeconds;
        public byte[] EncodedImageBytes;
        public int Width;
        public int Height;
        public string PixelFormat;
    }

    public struct ImuPayload
    {
        public double TimestampSeconds;
        public double X;
        public double Y;
        public double Z;
    }
}
