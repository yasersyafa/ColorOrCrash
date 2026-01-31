using System;
using System.Threading;
using ColorOrCrash.Features.Ball.Components;
using ColorOrCrash.Features.Ball.Models;
using ColorOrCrash.Global.Models;
using Cysharp.Threading.Tasks;
using NocturneThree.ServiceLocator;
using UnityEngine;
using UnityEngine.Pool;

namespace ColorOrCrash.Global.Components
{
    public enum GameState { Countdown, Playing, GameOver }

    [Service]
    public class GameManager : MonoBehaviour, IGameService
    {
        public GameSettings settings;

        private GameState _currentState = GameState.Playing;
        [HideInInspector] public bool isPaused = false;
        public GameState CurrentState => _currentState;
        public int Score { get; private set; }

        public event Action<GameState> OnGameStateChanged;
        public event Action<int, int> OnScoreAdded;
        public event Action OnGamePaused;

        private void Awake()
        {
            ServiceLocator.Register<GameManager>(this);
            ChangeState(GameState.Countdown);
        }

        public void ChangeState(GameState newState)
        {
            _currentState = newState;

            if (_currentState == GameState.Countdown)
            {
                PokiUnitySDK.Instance.gameplayStart();
                ResetGameState();
            }
            else if(_currentState == GameState.GameOver)
            {
                PokiUnitySDK.Instance.gameplayStop();
            }

            OnGameStateChanged?.Invoke(newState);
        }

        public void TogglePause()
        {
            PokiUnitySDK.Instance.gameplayStop();
            isPaused = true;
            Time.timeScale = 0;
            OnGamePaused?.Invoke();
        }

        private void ResetGameState()
        {
            Score = 0;
            OnScoreAdded?.Invoke(0, 0);

            var spawner = ServiceLocator.Get<BallSpawner>();
            if (spawner != null)
            {
                spawner.ClearAllBalls();
            }

            // 3. Audio BGM
            var audio = ServiceLocator.Get<AudioManager>();
            if (audio != null)
            {
                audio.PlayBGM("GameMusic");
            }

            Time.timeScale = 1;
            isPaused = false;
        }

        public void ResumeGame()
        {
            PokiUnitySDK.Instance.gameplayStart();
            isPaused = false;
            Time.timeScale = 1;
        }

        public void StartGame()
        {
            ChangeState(GameState.Playing);
        }

        void OnDestroy()
        {
            ServiceLocator.Unregister<GameManager>();
        }

        public void AddScore(int amount)
        {
            if (_currentState != GameState.Playing) return;
            Score += amount;
            OnScoreAdded?.Invoke(amount, Score);
        }
    }
}