using System;
using UnityEngine;
using ColorOrCrash.Vin.Core;

namespace ColorOrCrash.Vin.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private bool autoStartOnLoad = false;

        public GameSettings Settings => gameSettings;
        public GameState CurrentState { get; private set; } = GameState.Menu;
        public int Score { get; private set; }

        public event Action<GameState> OnGameStateChanged;
        public event Action<int> OnScoreChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (autoStartOnLoad)
            {
                StartGame();
            }
        }

        private void Update()
        {
            // Press J to start game from Menu or restart after Game Over
            if (Input.GetKeyDown(KeyCode.J))
            {
                if (CurrentState == GameState.Menu || CurrentState == GameState.GameOver)
                {
                    StartGame();
                }
            }

            // Press R to restart anytime
            if (Input.GetKeyDown(KeyCode.R) && CurrentState == GameState.GameOver)
            {
                StartGame();
            }
        }

        public void StartGame()
        {
            Score = 0;
            OnScoreChanged?.Invoke(Score);
            ChangeState(GameState.Playing);
        }

        public void PauseGame()
        {
            if (CurrentState == GameState.Playing)
            {
                Time.timeScale = 0f;
                ChangeState(GameState.Paused);
            }
        }

        public void ResumeGame()
        {
            if (CurrentState == GameState.Paused)
            {
                Time.timeScale = 1f;
                ChangeState(GameState.Playing);
            }
        }

        public void GameOver()
        {
            ChangeState(GameState.GameOver);
        }

        public void AddScore(int amount = 1)
        {
            Score += amount;
            OnScoreChanged?.Invoke(Score);
        }

        public void GoToMenu()
        {
            Time.timeScale = 1f;
            ChangeState(GameState.Menu);
        }

        private void ChangeState(GameState newState)
        {
            CurrentState = newState;
            OnGameStateChanged?.Invoke(newState);
        }
    }
}
