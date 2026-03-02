using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VinsMonoCapture.Capture;

namespace VinsMonoCapture.UI
{
    public static class RuntimeCaptureUiBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureCaptureUiExists()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = false;

            if (Object.FindObjectOfType<CaptureSessionController>() != null && Object.FindObjectOfType<CaptureStatusView>() != null)
            {
                return;
            }

            var controllerGo = new GameObject("CaptureSessionController");
            var controller = controllerGo.AddComponent<CaptureSessionController>();

            if (Object.FindObjectOfType<EventSystem>() == null)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }

            var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1170, 2532);

            var sessionInput = CreateInputField(canvasGo.transform, "会话名称", new Vector2(0, 500), "请输入会话名");
            var startButton = CreateButton(canvasGo.transform, "开始采集", new Vector2(-180, 420), "开始采集");
            var stopButton = CreateButton(canvasGo.transform, "停止采集", new Vector2(180, 420), "停止采集");

            var statusText = CreateText(canvasGo.transform, new Vector2(0, 320), "状态：待机");
            var frameText = CreateText(canvasGo.transform, new Vector2(0, 260), "图像帧数：0");
            var accelText = CreateText(canvasGo.transform, new Vector2(0, 200), "加速度样本：0");
            var gyroText = CreateText(canvasGo.transform, new Vector2(0, 140), "角速度样本：0");

            var statusViewGo = new GameObject("CaptureStatusView");
            var statusView = statusViewGo.AddComponent<CaptureStatusView>();
            statusView.Configure(controller, sessionInput, startButton, stopButton, statusText, frameText, accelText, gyroText);
        }

        private static InputField CreateInputField(Transform parent, string name, Vector2 position, string placeholderText)
        {
            var root = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(InputField));
            root.transform.SetParent(parent, false);
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(700, 80);
            rect.anchoredPosition = position;

            var text = CreateText(root.transform, Vector2.zero, "");
            text.alignment = TextAnchor.MiddleLeft;
            text.rectTransform.offsetMin = new Vector2(15, 0);
            text.rectTransform.offsetMax = new Vector2(-15, 0);

            var placeholder = CreateText(root.transform, Vector2.zero, placeholderText);
            placeholder.color = new Color(1f, 1f, 1f, 0.45f);
            placeholder.alignment = TextAnchor.MiddleLeft;
            placeholder.rectTransform.offsetMin = new Vector2(15, 0);
            placeholder.rectTransform.offsetMax = new Vector2(-15, 0);

            var inputField = root.GetComponent<InputField>();
            inputField.textComponent = text;
            inputField.placeholder = placeholder;
            return inputField;
        }

        private static Button CreateButton(Transform parent, string name, Vector2 position, string label)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(300, 90);
            rect.anchoredPosition = position;
            var text = CreateText(go.transform, Vector2.zero, label);
            text.alignment = TextAnchor.MiddleCenter;
            return go.GetComponent<Button>();
        }

        private static Text CreateText(Transform parent, Vector2 position, string content)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(900, 60);
            rect.anchoredPosition = position;

            var text = go.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 32;
            text.color = Color.white;
            text.text = content;
            return text;
        }
    }
}
