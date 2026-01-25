using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ColorOrCrash.Vin.Core;
using ColorOrCrash.Vin.Managers;

namespace ColorOrCrash.Vin.UI
{
    public class GameUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject gameOverPanel;

        [Header("Game Over")]
        [SerializeField] private TextMeshProUGUI finalScoreText;

        [Header("Buttons")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;

            // Setup buttons
            startButton?.onClick.AddListener(OnStartClicked);
            resumeButton?.onClick.AddListener(OnResumeClicked);
            restartButton?.onClick.AddListener(OnRestartClicked);
            menuButton?.onClick.AddListener(OnMenuClicked);
            quitButton?.onClick.AddListener(OnQuitClicked);

            // Show menu initially
            OnGameStateChanged(GameState.Menu);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
            }
        }

        private void Update()
        {
            // Pause with Escape
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameManager.Instance.CurrentState == GameState.Playing)
                {
                    GameManager.Instance.PauseGame();
                }
                else if (GameManager.Instance.CurrentState == GameState.Paused)
                {
                    GameManager.Instance.ResumeGame();
                }
            }
        }

        private void OnGameStateChanged(GameState newState)
        {
            menuPanel?.SetActive(newState == GameState.Menu);
            gameplayPanel?.SetActive(newState == GameState.Playing || newState == GameState.Paused);
            pausePanel?.SetActive(newState == GameState.Paused);
            gameOverPanel?.SetActive(newState == GameState.GameOver);

            if (newState == GameState.GameOver && finalScoreText != null)
            {
                finalScoreText.text = $"Final Score: {GameManager.Instance.Score}";
            }
        }

        private void OnStartClicked()
        {
            GameManager.Instance.StartGame();
        }

        private void OnResumeClicked()
        {
            GameManager.Instance.ResumeGame();
        }

        private void OnRestartClicked()
        {
            GameManager.Instance.StartGame();
        }

        private void OnMenuClicked()
        {
            GameManager.Instance.GoToMenu();
        }

        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
