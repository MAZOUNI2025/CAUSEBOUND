using System.Collections;
using Causebound.Core;
using Causebound.Save;
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
        [SerializeField, Min(1)] private int totalLevels = 5;
        [SerializeField] private Canvas menuCanvas;
        [SerializeField] private Camera menuCamera;

        private SaveService saveService;
        private bool isLoading;
        private Text progressText;

        private void Awake()
        {
            saveService = new SaveService();
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
            var scaler = menuCanvas.GetComponent<CanvasScaler>() ?? menuCanvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
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
                RefreshMenu();
                return;
            }

            var panel = CreateUiObject("MainMenuPanel", menuCanvas.transform);
            Stretch(panel);
            var panelImage = panel.gameObject.AddComponent<Image>();
            panelImage.color = new Color(0.025f, 0.04f, 0.08f, 1f);

            var title = CreateText("CAUSEBOUND", panel, 72, FontStyle.Bold);
            title.alignment = TextAnchor.MiddleCenter;
            title.color = new Color(0.85f, 0.94f, 1f, 1f);
            Anchor(title.rectTransform, new Vector2(0.08f, 0.82f), new Vector2(0.92f, 0.94f));

            var subtitle = CreateText("CAUSE → EFFECT PUZZLES", panel, 22, FontStyle.Normal);
            subtitle.alignment = TextAnchor.MiddleCenter;
            subtitle.color = new Color(0.45f, 0.75f, 0.9f, 1f);
            Anchor(subtitle.rectTransform, new Vector2(0.08f, 0.76f), new Vector2(0.92f, 0.82f));

            progressText = CreateText("PROGRESS", panel, 18, FontStyle.Normal);
            progressText.alignment = TextAnchor.MiddleCenter;
            progressText.color = new Color(0.65f, 0.72f, 0.84f, 1f);
            Anchor(progressText.rectTransform, new Vector2(0.08f, 0.70f), new Vector2(0.92f, 0.75f));

            var continueButton = CreateButton("ContinueButton", "CONTINUE", panel, 28);
            Anchor(continueButton.GetComponent<RectTransform>(), new Vector2(0.18f, 0.60f), new Vector2(0.82f, 0.68f));
            continueButton.onClick.AddListener(ContinueGame);

            var newGameButton = CreateButton("NewGameButton", "NEW GAME", panel, 24);
            Anchor(newGameButton.GetComponent<RectTransform>(), new Vector2(0.18f, 0.51f), new Vector2(0.82f, 0.58f));
            newGameButton.onClick.AddListener(StartNewGame);

            var levelLabel = CreateText("SELECT LEVEL", panel, 20, FontStyle.Bold);
            levelLabel.alignment = TextAnchor.MiddleCenter;
            levelLabel.color = Color.white;
            Anchor(levelLabel.rectTransform, new Vector2(0.08f, 0.44f), new Vector2(0.92f, 0.49f));

            var levelWidth = 0.16f;
            var levelGap = 0.02f;
            for (var index = 1; index <= totalLevels; index++)
            {
                var levelButton = CreateButton($"Level{index:00}Button", index.ToString("00"), panel, 20);
                var minX = 0.08f + (index - 1) * (levelWidth + levelGap);
                Anchor(levelButton.GetComponent<RectTransform>(), new Vector2(minX, 0.35f), new Vector2(minX + levelWidth, 0.42f));
                var selectedLevel = index;
                levelButton.onClick.AddListener(() => LoadLevel(selectedLevel));
            }

            var resetButton = CreateButton("ResetProgressButton", "RESET PROGRESS", panel, 18);
            Anchor(resetButton.GetComponent<RectTransform>(), new Vector2(0.28f, 0.20f), new Vector2(0.72f, 0.27f));
            resetButton.onClick.AddListener(ResetProgress);

            var footer = CreateText("Observe. Experiment. Reach the Exit.", panel, 18, FontStyle.Normal);
            footer.alignment = TextAnchor.MiddleCenter;
            footer.color = new Color(0.55f, 0.62f, 0.72f, 1f);
            Anchor(footer.rectTransform, new Vector2(0.08f, 0.10f), new Vector2(0.92f, 0.16f));
            RefreshMenu();
        }

        private void RefreshMenu()
        {
            var highest = Mathf.Clamp(saveService.LoadHighestUnlockedLevel(), 1, totalLevels);
            if (progressText != null)
            {
                progressText.text = $"{highest}/{totalLevels} LEVELS AVAILABLE";
            }

            var continueButton = menuCanvas.transform.Find("MainMenuPanel/ContinueButton")?.GetComponent<Button>();
            if (continueButton != null)
            {
                continueButton.interactable = !isLoading;
            }

            var newGameButton = menuCanvas.transform.Find("MainMenuPanel/NewGameButton")?.GetComponent<Button>();
            if (newGameButton != null)
            {
                newGameButton.interactable = !isLoading;
            }

            for (var index = 1; index <= totalLevels; index++)
            {
                var button = menuCanvas.transform.Find($"MainMenuPanel/Level{index:00}Button")?.GetComponent<Button>();
                if (button != null)
                {
                    button.interactable = !isLoading && index <= highest;
                }
            }
        }

        private void ContinueGame()
        {
            LoadLevel(Mathf.Clamp(saveService.LoadHighestUnlockedLevel(), 1, totalLevels));
        }

        private void StartNewGame()
        {
            saveService.ResetProgress();
            LoadLevel(1);
        }

        private void ResetProgress()
        {
            saveService.ResetProgress();
            RefreshMenu();
        }

        private void LoadLevel(int levelNumber)
        {
            if (isLoading || levelNumber < 1 || levelNumber > totalLevels || levelNumber > saveService.LoadHighestUnlockedLevel())
            {
                return;
            }

            isLoading = true;
            RefreshMenu();
            saveService.SaveLastLevel($"level-{levelNumber:00}");
            if (GameBootstrap.Instance != null)
            {
                GameBootstrap.Instance.SetState(GameState.LoadingLevel);
            }

            var sceneName = levelNumber == 1 ? levelSceneName : Causebound.Campaign.CampaignProgression.GetLevelSceneName(levelNumber);
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            if (operation == null)
            {
                Debug.LogError($"MainMenu could not load scene '{sceneName}'.", this);
                isLoading = false;
                RefreshMenu();
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

        private static Text CreateText(string value, Transform parent, int fontSize, FontStyle fontStyle)
        {
            var textObject = CreateUiObject(value, parent);
            var text = textObject.gameObject.AddComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }

        private static Button CreateButton(string objectName, string label, Transform parent, int fontSize)
        {
            var buttonRoot = CreateUiObject(objectName, parent);
            var image = buttonRoot.gameObject.AddComponent<Image>();
            image.color = new Color(0.1f, 0.48f, 0.72f, 1f);
            var button = buttonRoot.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            var text = CreateText(label, buttonRoot, fontSize, FontStyle.Bold);
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
