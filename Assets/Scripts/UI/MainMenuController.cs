using System.Collections;
using Causebound.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;

namespace Causebound.UI
{
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string levelSceneName = "Level01";
        [SerializeField] private Canvas menuCanvas;
        [SerializeField] private Camera menuCamera;

        private bool isLoading;

        private void Awake()
        {
            EnsureCamera();
            EnsureCanvas();
            EnsureEventSystem();
            BuildMenu();
        }

        private void EnsureCamera()
        {
            menuCamera ??= Camera.main;
            if (menuCamera != null)
            {
                menuCamera.enabled = true;
                return;
            }

            var cameraObject = new GameObject("MainMenuCamera");
            menuCamera = cameraObject.AddComponent<Camera>();
            menuCamera.tag = "MainCamera";
            menuCamera.transform.position = new Vector3(0f, 0f, -10f);
            menuCamera.transform.rotation = Quaternion.identity;
            menuCamera.clearFlags = CameraClearFlags.SolidColor;
            menuCamera.backgroundColor = new Color(0.025f, 0.04f, 0.08f, 1f);
            menuCamera.orthographic = true;
            menuCamera.orthographicSize = 5f;
        }

        private void EnsureCanvas()
        {
            menuCanvas ??= FindFirstObjectByType<Canvas>();
            if (menuCanvas == null)
            {
                var canvasObject = new GameObject("MainMenuCanvas");
                menuCanvas = canvasObject.AddComponent<Canvas>();
            }

            menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            if (menuCanvas.GetComponent<CanvasScaler>() == null)
            {
                var scaler = menuCanvas.gameObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080f, 1920f);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            }

            if (menuCanvas.GetComponent<GraphicRaycaster>() == null)
            {
                menuCanvas.gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        private static void EnsureEventSystem()
        {
            var eventSystem = FindFirstObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                var eventSystemObject = GameObject.Find("EventSystem") ?? new GameObject("EventSystem");
                eventSystem = eventSystemObject.AddComponent<EventSystem>();
            }

            if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }
        }

        private void BuildMenu()
        {
            if (menuCanvas.transform.Find("MainMenuPanel") != null)
            {
                return;
            }

            var panel = CreateUiObject("MainMenuPanel", menuCanvas.transform);
            Stretch(panel);
            var panelImage = panel.gameObject.AddComponent<Image>();
            panelImage.color = new Color(0.025f, 0.04f, 0.08f, 1f);

            var title = CreateText("CAUSEBOUND", panel, 72, FontStyle.Bold);
            title.alignment = TextAnchor.MiddleCenter;
            title.color = new Color(0.85f, 0.94f, 1f, 1f);
            Anchor(title.rectTransform, new Vector2(0.1f, 0.58f), new Vector2(0.9f, 0.82f));

            var subtitle = CreateText("CAUSE → EFFECT PUZZLES", panel, 22, FontStyle.Normal);
            subtitle.alignment = TextAnchor.MiddleCenter;
            subtitle.color = new Color(0.45f, 0.75f, 0.9f, 1f);
            Anchor(subtitle.rectTransform, new Vector2(0.1f, 0.49f), new Vector2(0.9f, 0.58f));

            var playButton = CreateButton("PLAY", panel);
            Anchor(playButton.GetComponent<RectTransform>(), new Vector2(0.22f, 0.34f), new Vector2(0.78f, 0.46f));
            playButton.onClick.AddListener(LoadLevel01);

            var footer = CreateText("Observe. Experiment. Reach the Exit.", panel, 18, FontStyle.Normal);
            footer.alignment = TextAnchor.MiddleCenter;
            footer.color = new Color(0.55f, 0.62f, 0.72f, 1f);
            Anchor(footer.rectTransform, new Vector2(0.1f, 0.18f), new Vector2(0.9f, 0.26f));
        }

        private void LoadLevel01()
        {
            if (isLoading)
            {
                return;
            }

            isLoading = true;
            if (GameBootstrap.Instance != null)
            {
                GameBootstrap.Instance.SetState(GameState.LoadingLevel);
            }

            StartCoroutine(LoadLevelAsync());
        }

        private IEnumerator LoadLevelAsync()
        {
            var operation = SceneManager.LoadSceneAsync(levelSceneName, LoadSceneMode.Single);
            if (operation == null)
            {
                Debug.LogError($"MainMenu could not load scene '{levelSceneName}'.");
                isLoading = false;
                yield break;
            }

            yield return operation;
        }

        private static RectTransform CreateUiObject(string objectName, Transform parent)
        {
            var objectRoot = new GameObject(objectName, typeof(RectTransform));
            objectRoot.transform.SetParent(parent, false);
            return objectRoot.GetComponent<RectTransform>();
        }

        private static Text CreateText(string value, RectTransform parent, int fontSize, FontStyle fontStyle)
        {
            var textObject = CreateUiObject(value, parent);
            var text = textObject.gameObject.AddComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.raycastTarget = false;
            return text;
        }

        private static Button CreateButton(string label, RectTransform parent)
        {
            var buttonRoot = CreateUiObject("PlayButton", parent);
            var image = buttonRoot.gameObject.AddComponent<Image>();
            image.color = new Color(0.1f, 0.48f, 0.72f, 1f);
            var button = buttonRoot.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            var text = CreateText(label, buttonRoot, 30, FontStyle.Bold);
            Stretch(text.rectTransform);
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            return button;
        }

        private static void Stretch(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        private static void Anchor(RectTransform rectTransform, Vector2 min, Vector2 max)
        {
            rectTransform.anchorMin = min;
            rectTransform.anchorMax = max;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

    }
}
