using System;

namespace VinsMonoCapture.Native
{
    public interface ICameraCaptureBridge
    {
        event Action<CameraFramePayload> FrameReceived;
        event Action<string> IntrinsicsReceived;

        void StartCameraCapture();
        void StopCameraCapture();
    }

    public interface IImuCaptureBridge
    {
        event Action<ImuPayload> AccelerometerSampleReceived;
        event Action<ImuPayload> GyroscopeSampleReceived;

        void StartImuCapture(double updateIntervalSeconds);
        void StopImuCapture();
    }
}
