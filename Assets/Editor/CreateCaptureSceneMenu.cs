#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VinsMonoCapture.Capture;
using VinsMonoCapture.UI;

public static class CreateCaptureSceneMenu
{
    [MenuItem("VinsMono/Create Default Capture Scene")]
    public static void CreateScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "CaptureScene";

        var controllerGo = new GameObject("CaptureSessionController");
        var controller = controllerGo.AddComponent<CaptureSessionController>();

        var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1170, 2532);

        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        var sessionInput = CreateInputField(canvasGo.transform, "SessionNameInput", new Vector2(0, 500), "session_name");
        var startButton = CreateButton(canvasGo.transform, "StartCaptureButton", new Vector2(-180, 420), "Start Capture");
        var stopButton = CreateButton(canvasGo.transform, "StopCaptureButton", new Vector2(180, 420), "Stop Capture");

        var statusText = CreateText(canvasGo.transform, "StatusText", new Vector2(0, 320), "Status: Idle");
        var frameText = CreateText(canvasGo.transform, "FrameCountText", new Vector2(0, 260), "Frames: 0");
        var accelText = CreateText(canvasGo.transform, "AccelCountText", new Vector2(0, 200), "Accel Samples: 0");
        var gyroText = CreateText(canvasGo.transform, "GyroCountText", new Vector2(0, 140), "Gyro Samples: 0");

        var statusViewGo = new GameObject("CaptureStatusView");
        var statusView = statusViewGo.AddComponent<CaptureStatusView>();

        AssignSerializedField(statusView, "captureSessionController", controller);
        AssignSerializedField(statusView, "sessionNameInputField", sessionInput);
        AssignSerializedField(statusView, "startCaptureButton", startButton);
        AssignSerializedField(statusView, "stopCaptureButton", stopButton);
        AssignSerializedField(statusView, "statusText", statusText);
        AssignSerializedField(statusView, "frameCounterText", frameText);
        AssignSerializedField(statusView, "accelerometerCounterText", accelText);
        AssignSerializedField(statusView, "gyroscopeCounterText", gyroText);

        var sceneDirectory = "Assets/Scenes";
        if (!AssetDatabase.IsValidFolder(sceneDirectory))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
        }

        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "Assets/Scenes/CaptureScene.unity");
        AssetDatabase.SaveAssets();
        Debug.Log("Capture scene created at Assets/Scenes/CaptureScene.unity");
    }

    private static void AssignSerializedField(Object targetObject, string fieldName, Object value)
    {
        var serializedObject = new SerializedObject(targetObject);
        var property = serializedObject.FindProperty(fieldName);
        property.objectReferenceValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static InputField CreateInputField(Transform parent, string name, Vector2 anchoredPosition, string placeholder)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(InputField));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(700, 80);
        rect.anchoredPosition = anchoredPosition;

        var text = CreateText(go.transform, "Text", Vector2.zero, "");
        text.alignment = TextAnchor.MiddleLeft;
        text.rectTransform.offsetMin = new Vector2(15, 0);
        text.rectTransform.offsetMax = new Vector2(-15, 0);

        var placeholderText = CreateText(go.transform, "Placeholder", Vector2.zero, placeholder);
        placeholderText.color = new Color(1, 1, 1, 0.45f);
        placeholderText.alignment = TextAnchor.MiddleLeft;
        placeholderText.rectTransform.offsetMin = new Vector2(15, 0);
        placeholderText.rectTransform.offsetMax = new Vector2(-15, 0);

        var inputField = go.GetComponent<InputField>();
        inputField.textComponent = text;
        inputField.placeholder = placeholderText;
        return inputField;
    }

    private static Button CreateButton(Transform parent, string name, Vector2 anchoredPosition, string label)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 90);
        rect.anchoredPosition = anchoredPosition;

        var text = CreateText(go.transform, "Label", Vector2.zero, label);
        text.alignment = TextAnchor.MiddleCenter;

        return go.GetComponent<Button>();
    }

    private static Text CreateText(Transform parent, string name, Vector2 anchoredPosition, string content)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(900, 60);
        rect.anchoredPosition = anchoredPosition;

        var text = go.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 32;
        text.color = Color.white;
        text.text = content;
        return text;
    }
}
#endif
