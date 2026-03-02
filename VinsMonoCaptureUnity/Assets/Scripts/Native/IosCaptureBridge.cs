using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace VinsMonoCapture.Native
{
    public class IosCaptureBridge : ICameraCaptureBridge, IImuCaptureBridge
    {
        public event Action<CameraFramePayload>? FrameReceived;
        public event Action<string>? IntrinsicsReceived;
        public event Action<ImuPayload>? AccelerometerSampleReceived;
        public event Action<ImuPayload>? GyroscopeSampleReceived;

        private static IosCaptureBridge? activeInstance;

#if UNITY_IOS && !UNITY_EDITOR
        private delegate void CameraFrameCallback(long frameIndex, double timestampSeconds, IntPtr imageBytes, int imageLength, int width, int height);
        private delegate void ImuCallback(double timestampSeconds, double x, double y, double z);
        private delegate void StringCallback(IntPtr cString);

        [DllImport("__Internal")] private static extern void VinsStartCameraCapture(CameraFrameCallback frameCallback, StringCallback intrinsicsCallback);
        [DllImport("__Internal")] private static extern void VinsStopCameraCapture();
        [DllImport("__Internal")] private static extern void VinsStartImuCapture(double updateIntervalSeconds, ImuCallback accelerometerCallback, ImuCallback gyroscopeCallback);
        [DllImport("__Internal")] private static extern void VinsStopImuCapture();
#endif

        public void StartCameraCapture()
        {
            activeInstance = this;
#if UNITY_IOS && !UNITY_EDITOR
            VinsStartCameraCapture(OnNativeFrame, OnNativeIntrinsics);
#else
            Debug.Log("StartCameraCapture called in non-iOS environment");
#endif
        }

        public void StopCameraCapture()
        {
#if UNITY_IOS && !UNITY_EDITOR
            VinsStopCameraCapture();
#endif
        }

        public void StartImuCapture(double updateIntervalSeconds)
        {
            activeInstance = this;
#if UNITY_IOS && !UNITY_EDITOR
            VinsStartImuCapture(updateIntervalSeconds, OnNativeAccelerometer, OnNativeGyroscope);
#endif
        }

        public void StopImuCapture()
        {
#if UNITY_IOS && !UNITY_EDITOR
            VinsStopImuCapture();
#endif
        }

#if UNITY_IOS && !UNITY_EDITOR
        [AOT.MonoPInvokeCallback(typeof(CameraFrameCallback))]
        private static void OnNativeFrame(long frameIndex, double timestampSeconds, IntPtr imageBytes, int imageLength, int width, int height)
        {
            if (activeInstance == null || imageBytes == IntPtr.Zero || imageLength <= 0) return;

            var managedBytes = new byte[imageLength];
            Marshal.Copy(imageBytes, managedBytes, 0, imageLength);

            activeInstance.FrameReceived?.Invoke(new CameraFramePayload
            {
                FrameIndex = frameIndex,
                TimestampSeconds = timestampSeconds,
                EncodedImageBytes = managedBytes,
                Width = width,
                Height = height,
                PixelFormat = "jpeg"
            });
        }

        [AOT.MonoPInvokeCallback(typeof(ImuCallback))]
        private static void OnNativeAccelerometer(double timestampSeconds, double x, double y, double z)
        {
            activeInstance?.AccelerometerSampleReceived?.Invoke(new ImuPayload { TimestampSeconds = timestampSeconds, X = x, Y = y, Z = z });
        }

        [AOT.MonoPInvokeCallback(typeof(ImuCallback))]
        private static void OnNativeGyroscope(double timestampSeconds, double x, double y, double z)
        {
            activeInstance?.GyroscopeSampleReceived?.Invoke(new ImuPayload { TimestampSeconds = timestampSeconds, X = x, Y = y, Z = z });
        }

        [AOT.MonoPInvokeCallback(typeof(StringCallback))]
        private static void OnNativeIntrinsics(IntPtr cString)
        {
            if (activeInstance == null || cString == IntPtr.Zero) return;
            var intrinsicsJson = Marshal.PtrToStringAnsi(cString) ?? string.Empty;
            activeInstance.IntrinsicsReceived?.Invoke(intrinsicsJson);
        }
#endif
    }
}
