using System;
using System.Collections;
using Causebound.Core;
using Causebound.Levels;
using Causebound.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Causebound.Campaign
{
    public sealed class CampaignProgression : MonoBehaviour
    {
        [SerializeField, Min(1)] private int totalLevels = 5;
        [SerializeField] private LevelCompletionState completion;
        [SerializeField] private LevelResetController resetController;
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        private SaveService saveService;
        private bool transitionStarted;
        private int currentLevel;

        public int CurrentLevel => currentLevel > 0 ? currentLevel : ParseLevelNumber(SceneManager.GetActiveScene().name);
        public int TotalLevels => Mathf.Max(1, totalLevels);
        public bool IsFinalLevel => CurrentLevel >= TotalLevels;
        public bool IsComplete => completion != null && completion.IsComplete;
        public event Action Completed;

        private void Awake()
        {
            saveService = new SaveService();
            currentLevel = ParseLevelNumber(SceneManager.GetActiveScene().name);
            completion ??= FindFirstObjectByType<LevelCompletionState>();
            resetController ??= FindFirstObjectByType<LevelResetController>();
        }

        private void OnEnable()
        {
            if (completion != null)
            {
                completion.Completed += HandleCompleted;
            }
        }

        private void Start()
        {
            if (completion != null && completion.IsComplete)
            {
                HandleCompleted();
            }
        }

        private void OnDisable()
        {
            if (completion != null)
            {
                completion.Completed -= HandleCompleted;
            }
        }

        public void ResetLevel()
        {
            transitionStarted = false;
            resetController?.ResetLevel();
        }

        public void LoadNextLevel()
        {
            if (transitionStarted)
            {
                return;
            }

            transitionStarted = true;
            var targetLevel = IsFinalLevel ? 1 : CurrentLevel + 1;
            var targetScene = IsFinalLevel ? mainMenuSceneName : GetLevelSceneName(targetLevel);
            StartCoroutine(LoadSceneAsync(targetScene));
        }

        public void ReplayCurrentLevel()
        {
            if (transitionStarted)
            {
                return;
            }

            transitionStarted = true;
            StartCoroutine(LoadSceneAsync(SceneManager.GetActiveScene().name));
        }

        public void ReturnToMainMenu()
        {
            if (transitionStarted)
            {
                return;
            }

            transitionStarted = true;
            StartCoroutine(LoadSceneAsync(mainMenuSceneName));
        }

        public void ResetCampaignProgress()
        {
            saveService.ResetProgress();
        }

        public bool IsLevelUnlocked(int levelNumber)
        {
            return levelNumber >= 1 && levelNumber <= saveService.LoadHighestUnlockedLevel();
        }

        public void LoadLevel(int levelNumber)
        {
            if (transitionStarted || !IsLevelUnlocked(levelNumber))
            {
                return;
            }

            transitionStarted = true;
            saveService.SaveLastLevel($"level-{levelNumber:00}");
            StartCoroutine(LoadSceneAsync(GetLevelSceneName(levelNumber)));
        }

        private void HandleCompleted()
        {
            if (IsFinalLevel)
            {
                saveService.UnlockLevel(CurrentLevel);
            }
            else
            {
                saveService.UnlockLevel(CurrentLevel + 1);
            }

            Completed?.Invoke();
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            if (GameBootstrap.Instance != null)
            {
                GameBootstrap.Instance.SetState(GameState.LoadingLevel);
            }

            var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            if (operation == null)
            {
                transitionStarted = false;
                Debug.LogError($"CAUSEBOUND could not load scene '{sceneName}'.", this);
                yield break;
            }

            yield return operation;
            if (GameBootstrap.Instance != null)
            {
                var activeSceneName = SceneManager.GetActiveScene().name;
                GameBootstrap.Instance.SetState(activeSceneName.StartsWith("Level", StringComparison.OrdinalIgnoreCase) ? GameState.Gameplay : GameState.MainMenu);
            }
        }

        public static string GetLevelSceneName(int levelNumber)
        {
            return $"Level{Mathf.Max(1, levelNumber):00}";
        }

        private static int ParseLevelNumber(string sceneName)
        {
            if (sceneName.StartsWith("Level", StringComparison.OrdinalIgnoreCase) && int.TryParse(sceneName.Substring(5), out var number))
            {
                return Mathf.Max(1, number);
            }

            return 1;
        }
    }
}
