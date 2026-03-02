using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VinsMonoCapture.Capture;

namespace VinsMonoCapture.UI
{
    public class CaptureStatusView : MonoBehaviour
    {
        [SerializeField] private CaptureSessionController captureSessionController = null!;
        [SerializeField] private TMP_InputField sessionNameInputField = null!;
        [SerializeField] private Button startCaptureButton = null!;
        [SerializeField] private Button stopCaptureButton = null!;
        [SerializeField] private TMP_Text statusText = null!;
        [SerializeField] private TMP_Text frameCounterText = null!;
        [SerializeField] private TMP_Text accelerometerCounterText = null!;
        [SerializeField] private TMP_Text gyroscopeCounterText = null!;

        private void Awake()
        {
            startCaptureButton.onClick.AddListener(HandleStartCaptureClicked);
            stopCaptureButton.onClick.AddListener(captureSessionController.StopCapture);
            captureSessionController.StatusUpdated += OnStatusUpdated;
            OnStatusUpdated(0, 0, 0, "Idle");
        }

        private void HandleStartCaptureClicked()
        {
            captureSessionController.SetSessionName(sessionNameInputField.text);
            captureSessionController.StartCapture();
        }

        private void OnStatusUpdated(int frameCount, int accelerometerCount, int gyroscopeCount, string statusMessage)
        {
            statusText.text = $"Status: {statusMessage}";
            frameCounterText.text = $"Frames: {frameCount}";
            accelerometerCounterText.text = $"Accel Samples: {accelerometerCount}";
            gyroscopeCounterText.text = $"Gyro Samples: {gyroscopeCount}";
        }
    }
}
