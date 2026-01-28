using UnityEngine;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;
using System.Threading;
using ColorOrCrash.Features.Ball.Components;
using ColorOrCrash.Features.Ball.Models;
using NocturneThree.ServiceLocator;
using ColorOrCrash.Global.Models;
using System;

namespace ColorOrCrash.Global.Components
{
    public enum GameState { Playing, GameOver }

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

        private void Start()
        {
            ChangeState(GameState.Playing);
            if(_currentState == GameState.Playing)
            {
                ServiceLocator.Get<AudioManager>().PlayBGM("GameMusic");
            }
        }

        public void ChangeState(GameState newState)
        {
            _currentState = newState;
            OnGameStateChanged?.Invoke(newState);
        }

        public void TogglePause()
        {
            isPaused = true;
            Time.timeScale = 0;
            OnGamePaused?.Invoke();
        }

        public void ResumeGame()
        {
            isPaused = false;
            Time.timeScale = 1;
        }

        void OnDestroy()
        {
            ServiceLocator.Unregister<GameManager>();
        }

        public void AddScore(int amount)
        {
            Score += amount;

            OnScoreAdded?.Invoke(amount, Score);
        }
    }
}