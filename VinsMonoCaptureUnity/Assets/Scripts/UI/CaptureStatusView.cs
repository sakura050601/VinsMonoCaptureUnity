using UnityEngine;
using UnityEngine.UI;
using VinsMonoCapture.Capture;

namespace VinsMonoCapture.UI
{
    public class CaptureStatusView : MonoBehaviour
    {
        [SerializeField] private CaptureSessionController captureSessionController = null!;
        [SerializeField] private InputField sessionNameInputField = null!;
        [SerializeField] private Button startCaptureButton = null!;
        [SerializeField] private Button stopCaptureButton = null!;
        [SerializeField] private Text statusText = null!;
        [SerializeField] private Text frameCounterText = null!;
        [SerializeField] private Text accelerometerCounterText = null!;
        [SerializeField] private Text gyroscopeCounterText = null!;

        public void Configure(
            CaptureSessionController controller,
            InputField sessionInput,
            Button startButton,
            Button stopButton,
            Text statusLabel,
            Text frameLabel,
            Text accelerometerLabel,
            Text gyroscopeLabel)
        {
            captureSessionController = controller;
            sessionNameInputField = sessionInput;
            startCaptureButton = startButton;
            stopCaptureButton = stopButton;
            statusText = statusLabel;
            frameCounterText = frameLabel;
            accelerometerCounterText = accelerometerLabel;
            gyroscopeCounterText = gyroscopeLabel;
        }

        private void Awake()
        {
            startCaptureButton.onClick.AddListener(HandleStartCaptureClicked);
            stopCaptureButton.onClick.AddListener(captureSessionController.StopCapture);
            captureSessionController.StatusUpdated += OnStatusUpdated;
            OnStatusUpdated(0, 0, 0, "待机");
        }

        private void HandleStartCaptureClicked()
        {
            captureSessionController.SetSessionName(sessionNameInputField.text);
            captureSessionController.StartCapture();
        }

        private void OnStatusUpdated(int frameCount, int accelerometerCount, int gyroscopeCount, string statusMessage)
        {
            statusText.text = $"状态：{statusMessage}";
            frameCounterText.text = $"图像帧数：{frameCount}";
            accelerometerCounterText.text = $"加速度样本：{accelerometerCount}";
            gyroscopeCounterText.text = $"角速度样本：{gyroscopeCount}";
        }
    }
}
