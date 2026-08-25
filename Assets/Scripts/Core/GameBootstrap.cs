using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Causebound.Core
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        public static GameBootstrap Instance { get; private set; }
        public GameState CurrentState { get; private set; } = GameState.Booting;

        [SerializeField] private string mainMenuSceneName = "MainMenu";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            StartCoroutine(LoadMainMenu());
        }

        private IEnumerator LoadMainMenu()
        {
            if (SceneManager.GetActiveScene().name == mainMenuSceneName)
            {
                SetState(GameState.MainMenu);
                yield break;
            }

            SetState(GameState.LoadingLevel);
            var operation = SceneManager.LoadSceneAsync(mainMenuSceneName, LoadSceneMode.Single);
            if (operation == null)
            {
                Debug.LogError($"CAUSEBOUND bootstrap could not load scene '{mainMenuSceneName}'.");
                yield break;
            }

            yield return operation;
            SetState(GameState.MainMenu);
        }

        public void SetState(GameState nextState)
        {
            CurrentState = nextState;
        }
    }
}
