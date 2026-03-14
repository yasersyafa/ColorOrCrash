using System;
using ColorOrCrash.Features.Achievement.Events;
using ColorOrCrash.Features.Ball.Components;
using ColorOrCrash.Features.LootLocker.Services;
using ColorOrCrash.Vin.Core;
using NocturneThree.EventSystem;
using NocturneThree.ServiceLocator;
using UnityEngine;

namespace ColorOrCrash.Global.Components
{
    public enum GameState { Countdown, Playing, GameOver }

    [Service]
    public class GameManager : MonoBehaviour, IGameService
    {
        public Models.GameSettings settings;

        private GameState _currentState = GameState.Playing;
        [HideInInspector] public bool isPaused = false;
        public GameState CurrentState => _currentState;

#region Score
        public int Score { get; private set; }
        public int RedPoint { get; private set; }
        public int BluePoint { get; private set; }
        public int GreenPoint { get; private set; }
#endregion
        public event Action<GameState> OnGameStateChanged;
        public event Action<int, int> OnScoreAdded;
        public event Action OnGamePaused;

        private SaveManager _saveManager;
        private AudioManager _audioManager;
        private BallSpawner _ballSpawner;

        private void Start()
        {
            _saveManager = ServiceLocator.Get<SaveManager>();
            _audioManager = ServiceLocator.Get<AudioManager>();
            _ballSpawner = ServiceLocator.Get<BallSpawner>();
            ChangeState(GameState.Countdown);
        }

        public void ChangeState(GameState newState)
        {
            _currentState = newState;

            if (_currentState == GameState.Countdown)
            {
                ResetGameState();
            }
            else if(_currentState == GameState.GameOver)
            {

                if(Score >= _saveManager.Data.highScore)
                {
                    _saveManager.Data.highScore = Score;
                    
                    string memberId = PlayerPrefs.GetString(LeaderboardService.memberKey, "");
                    LeaderboardService.TrySubmitScore(memberId, Score);
                }
            }

            OnGameStateChanged?.Invoke(newState);
        }

        public void TogglePause()
        {
            isPaused = true;
            Time.timeScale = 0;
            OnGamePaused?.Invoke();
        }

        private void ResetGameState()
        {
            Score = 0;
            RedPoint = 0;
            BluePoint = 0;
            GreenPoint = 0;
            OnScoreAdded?.Invoke(0, 0);

            var spawner = _ballSpawner;
            if (spawner != null)
            {
                spawner.ClearAllBalls();
            }

            // 3. Audio BGM
            var audio = _audioManager;
            if (audio != null)
            {
                audio.PlayBGM("GameMusic");
            }

            Time.timeScale = 1;
            isPaused = false;
        }

        public void ResumeGame()
        {
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

        public void AddPoint(GameColor color)
        {
            switch (color)
            {
                case GameColor.Red:
                    RedPoint++;
                    _saveManager.Data.redCoins += RedPoint;
                    EventBus.Publish(new ProgressUpdateEvent(Features.Achievement.Models.MissionType.EatObstacle, 1, "Red"));
                    break;
                case GameColor.Green:
                    GreenPoint++;
                    _saveManager.Data.greenCoins += GreenPoint;
                    EventBus.Publish(new ProgressUpdateEvent(Features.Achievement.Models.MissionType.EatObstacle, 1, "Green"));
                    break;
                case GameColor.Blue:
                    BluePoint++;
                    _saveManager.Data.blueCoins += BluePoint;
                    EventBus.Publish(new ProgressUpdateEvent(Features.Achievement.Models.MissionType.EatObstacle, 1, "Blue"));
                    break;
                default:
                    break;
            }
        }
    }
}