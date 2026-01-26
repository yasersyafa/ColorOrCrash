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
        public static GameManager Instance;
        public GameSettings settings;
        
        private GameState _currentState = GameState.Playing;
        public GameState CurrentState => _currentState;
        public int Score { get; private set; }

        public event Action<GameState> OnGameStateChanged;

        private void Awake()
        {
            if(Instance == null)
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
            ChangeState(GameState.Playing);
        }

        public void ChangeState(GameState newState)
        {
            _currentState = newState;
            OnGameStateChanged?.Invoke(newState);
        }

        public void AddScore(int amount)
        {
            Score += amount;
        }
    }
}