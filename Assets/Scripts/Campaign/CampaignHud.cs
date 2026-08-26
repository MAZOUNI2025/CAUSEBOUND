using System;
using Causebound.Levels;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Causebound.Campaign
{
    public sealed class CampaignHud : MonoBehaviour
    {
        [SerializeField] private CampaignProgression progression;
        [SerializeField] private LevelData levelData;
        [SerializeField] private LevelCompletionState completion;
        [SerializeField] private Canvas canvas;

        private Text levelText;
        private Text objectiveText;
        private GameObject pauseOverlay;
        private GameObject helpOverlay;
        private GameObject completionOverlay;
        private bool paused;

        private void Awake()
        {
            progression ??= FindFirstObjectByType<CampaignProgression>();
            completion ??= FindFirstObjectByType<LevelCompletionState>();
            EnsureCanvas();
            EnsureEventSystem();
        }

        private void OnEnable()
        {
            if (progression != null)
            {
                progression.Completed += HandleCompleted;
            }
        }

        private void Start()
        {
            BuildHud();
            UpdateLabels();
            if (completion != null && completion.IsComplete)
            {
                HandleCompleted();
            }
        }

        private void OnDisable()
        {
            if (progression != null)
            {
                progression.Completed -= HandleCompleted;
            }
            Time.timeScale = 1f;
        }

        public void ToggleHints()
        {
            if (helpOverlay != null)
            {
                helpOverlay.SetActive(!helpOverlay.activeSelf);
            }
        }

        public void TogglePause()
        {
            if (completionOverlay != null && completionOverlay.activeSelf)
            {
                return;
            }

            paused = !paused;
            Time.timeScale = paused ? 0f : 1f;
            if (pauseOverlay != null)
            {
                pauseOverlay.SetActive(paused);
            }
        }

        private void EnsureCanvas()
        {
            canvas ??= FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                var canvasObject = new GameObject("CampaignCanvas");
                canvas = canvasObject.AddComponent<Canvas>();
            }

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.GetComponent<CanvasScaler>() ?? canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            if (canvas.GetComponent<GraphicRaycaster>() == null)
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        private static void EnsureEventSystem()
        {
            var eventSystem = FindFirstObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                eventSystem = (GameObject.Find("EventSystem") ?? new GameObject("EventSystem")).AddComponent<EventSystem>();
            }

            if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }
        }

        private void BuildHud()
        {
            if (canvas.transform.Find("CampaignHudRoot") != null)
            {
                return;
            }

            var root = CreateRect("CampaignHudRoot", canvas.transform);
            Stretch(root);

            var panel = CreateRect("InfoPanel", root);
            Anchor(panel, new Vector2(0.04f, 0.78f), new Vector2(0.96f, 0.97f));
            var panelImage = panel.gameObject.AddComponent<Image>();
            panelImage.color = new Color(0.02f, 0.04f, 0.08f, 0.92f);

            levelText = CreateText("LEVEL", panel, 30, FontStyle.Bold);
            Anchor(levelText.rectTransform, new Vector2(0.04f, 0.55f), new Vector2(0.68f, 0.93f));
            levelText.color = new Color(0.75f, 0.92f, 1f, 1f);

            objectiveText = CreateText("OBJECTIVE", panel, 20, FontStyle.Normal);
            Anchor(objectiveText.rectTransform, new Vector2(0.04f, 0.08f), new Vector2(0.72f, 0.52f));
            objectiveText.color = Color.white;

            var resetButton = CreateButton("RESET", panel, 22);
            Anchor(resetButton.GetComponent<RectTransform>(), new Vector2(0.74f, 0.52f), new Vector2(0.96f, 0.91f));
            resetButton.onClick.AddListener(() => progression?.ResetLevel());

            var helpButton = CreateButton("HINTS", panel, 22);
            Anchor(helpButton.GetComponent<RectTransform>(), new Vector2(0.74f, 0.31f), new Vector2(0.96f, 0.49f));
            helpButton.onClick.AddListener(ToggleHints);

            var pauseButton = CreateButton("PAUSE", panel, 22);
            Anchor(pauseButton.GetComponent<RectTransform>(), new Vector2(0.74f, 0.09f), new Vector2(0.96f, 0.27f));
            pauseButton.onClick.AddListener(TogglePause);

            pauseOverlay = CreateOverlay("PauseOverlay", root, "PAUSED", "Continue when you are ready.", "RESUME");
            pauseOverlay.GetComponentInChildren<Button>().onClick.AddListener(TogglePause);
            pauseOverlay.SetActive(false);

            var hints = levelData == null || levelData.Hints == null || levelData.Hints.Count == 0
                ? "Observe each state before you act."
                : string.Join(Environment.NewLine, levelData.Hints);
            helpOverlay = CreateOverlay("HelpOverlay", root, "HINTS", hints, "CLOSE");
            helpOverlay.GetComponentInChildren<Button>().onClick.AddListener(ToggleHints);
            helpOverlay.SetActive(false);

            completionOverlay = CreateOverlay("CompletionOverlay", root, "LEVEL COMPLETE", "The chain is understood. Choose your next step.", progression != null && progression.IsFinalLevel ? "FINISH" : "NEXT LEVEL");
            var completionButton = completionOverlay.GetComponentInChildren<Button>();
            completionButton.onClick.AddListener(() => progression?.LoadNextLevel());
            var replayButton = CreateButton("REPLAY", completionOverlay.transform as RectTransform, 22);
            Anchor(replayButton.GetComponent<RectTransform>(), new Vector2(0.25f, 0.17f), new Vector2(0.75f, 0.27f));
            replayButton.onClick.AddListener(() => progression?.ReplayCurrentLevel());
            var menuButton = CreateButton("MAIN MENU", completionOverlay.transform as RectTransform, 22);
            Anchor(menuButton.GetComponent<RectTransform>(), new Vector2(0.25f, 0.06f), new Vector2(0.75f, 0.16f));
            menuButton.onClick.AddListener(() => progression?.ReturnToMainMenu());
            completionOverlay.SetActive(false);
        }

        private void UpdateLabels()
        {
            if (progression == null)
            {
                return;
            }

            if (levelText != null)
            {
                levelText.text = $"CAUSEBOUND  •  LEVEL {progression.CurrentLevel}/{progression.TotalLevels}";
            }
            if (objectiveText != null)
            {
                objectiveText.text = levelData == null || string.IsNullOrWhiteSpace(levelData.Objective) ? "REACH THE EXIT" : levelData.Objective;
            }
        }

        private void HandleCompleted()
        {
            if (completionOverlay != null)
            {
                completionOverlay.SetActive(true);
            }
        }

        private static GameObject CreateOverlay(string name, RectTransform parent, string title, string body, string primaryLabel)
        {
            var overlay = CreateRect(name, parent);
            Stretch(overlay);
            var image = overlay.gameObject.AddComponent<Image>();
            image.color = new Color(0.01f, 0.02f, 0.05f, 0.96f);
            var titleText = CreateText(title, overlay, 52, FontStyle.Bold);
            Anchor(titleText.rectTransform, new Vector2(0.08f, 0.57f), new Vector2(0.92f, 0.76f));
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = new Color(0.6f, 0.95f, 1f, 1f);
            var bodyText = CreateText(body, overlay, 22, FontStyle.Normal);
            Anchor(bodyText.rectTransform, new Vector2(0.12f, 0.47f), new Vector2(0.88f, 0.58f));
            bodyText.alignment = TextAnchor.MiddleCenter;
            bodyText.color = Color.white;
            var primary = CreateButton(primaryLabel, overlay, 26);
            Anchor(primary.GetComponent<RectTransform>(), new Vector2(0.25f, 0.29f), new Vector2(0.75f, 0.39f));
            return overlay.gameObject;
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var objectRoot = new GameObject(name, typeof(RectTransform));
            objectRoot.transform.SetParent(parent, false);
            return objectRoot.GetComponent<RectTransform>();
        }

        private static Text CreateText(string value, Transform parent, int size, FontStyle style)
        {
            var text = CreateRect(value, parent).gameObject.AddComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.fontStyle = style;
            text.raycastTarget = false;
            return text;
        }

        private static Button CreateButton(string label, Transform parent, int size)
        {
            var root = CreateRect(label, parent);
            var image = root.gameObject.AddComponent<Image>();
            image.color = new Color(0.08f, 0.45f, 0.68f, 1f);
            var button = root.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            var text = CreateText(label, root, size, FontStyle.Bold);
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
