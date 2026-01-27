using System.Collections.Generic;
using System.Threading;
using ColorOfCrash.Utils;
using ColorOrCrash.Features.Ball.Models;
using ColorOrCrash.Features.Player.Components;
using ColorOrCrash.Global.Components;
using ColorOrCrash.Vin.Core;
using Cysharp.Threading.Tasks;
using NocturneThree.ServiceLocator;
using UnityEngine;
using UnityEngine.Pool;
using Global = ColorOrCrash.Global;

namespace ColorOrCrash.Features.Ball.Components
{
    [Service]
    public class BallSpawner : MonoBehaviour, IGameService
    {
        public BallSpawnerConfig config;
        private int score;
        [Header("References")]
        [SerializeField] private BallController ballPrefab;
        [SerializeField] private Transform ballContainer;
        [SerializeField] private PlayerController playerController;
        
        [Header("Wall References")]
        [SerializeField] private Transform topWall;
        [SerializeField] private Transform bottomWall;
        [SerializeField] private Transform leftWall;
        [SerializeField] private Transform rightWall;
        [SerializeField] private float spawnOffset = 1.5f;

        private Global.Models.GameSettings _settings;
        private IObjectPool<BallController> _ballPool;
        private CancellationTokenSource _spawnCts;
        private readonly List<BallController> _activeBalls = new();
        private GameManager manager;

        private void Awake()
        {
            // Industry Best Practice: Gunakan Object Pool untuk performa tinggi
            _ballPool = new ObjectPool<BallController>(
                createFunc: () => Instantiate(ballPrefab, ballContainer),
                actionOnGet: ball => ball.gameObject.SetActive(true),
                actionOnRelease: ball => ball.gameObject.SetActive(false),
                actionOnDestroy: ball => Destroy(ball.gameObject),
                defaultCapacity: 10,
                maxSize: 20
            );
        }

        private void Start()
        {
            manager = ServiceLocator.Get<GameManager>();
            manager.OnGameStateChanged += HandleGameStateChanged;

            _settings = manager.settings;

            // if the scripts was race condition, run it manually
            if (manager.CurrentState == Global.Components.GameState.Playing)
            {
                HandleGameStateChanged(Global.Components.GameState.Playing);
            }
        }

        private void OnDestroy()
        {

            StopSpawning();
            if (manager != null)
                manager.OnGameStateChanged -= HandleGameStateChanged;
            ServiceLocator.Unregister<BallSpawner>();
        }

        private void HandleGameStateChanged(Global.Components.GameState newState)
        {
            StopSpawning();

            if (newState == Global.Components.GameState.Playing)
            {
                ClearAllBalls();
                _spawnCts = new CancellationTokenSource();
                SpawnLoopAsync(_spawnCts.Token).Forget();
            }
        }

        private async UniTaskVoid SpawnLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_activeBalls.Count < config.maxBallsInScene)
                {
                    SpawnBall();
                }

                float interval = CalculateCurrentInterval();
                await UniTask.Delay(System.TimeSpan.FromSeconds(interval), cancellationToken: token);
            }
        }

        private void SpawnBall()
        {
            Vector2 spawnPos = GetSpawnTopPositionOutsidePlayspace();
            Vector2 direction = GetRandomDirectionTowardsPlayspace(spawnPos);
            GameColor ballColor = GetBiasedRandomColor();
            float speed = Random.Range(config.ballMinSpeed, config.ballMaxSpeed);

            BallController ball = _ballPool.Get();
            if(ball == null) Debug.LogWarning("Ball Pool returned null!");
            ball.transform.SetPositionAndRotation(spawnPos, Quaternion.identity);
            
            ball.Initialize(ballColor, direction, speed, _ballPool);
            _activeBalls.Add(ball);
        }

        private GameColor GetBiasedRandomColor()
        {
            // int score = GameManager.Instance.Score;
            float matchingChance = Mathf.Max(_settings.minMatchingColorChance, 
                _settings.startMatchingColorChance - (score * _settings.matchingChanceDecreasePerScore));
            
            GameColor playerColor = playerController != null ? playerController.CurrentType : EnumUtils.GetRandomEnumValue<GameColor>();
            
            if (Random.value < matchingChance) return playerColor;

            // Dapatkan warna selain warna player
            GameColor otherColor;
            do {
                otherColor = (GameColor)Random.Range(0, 3);
            } while (otherColor == playerColor);
            
            return otherColor;
        }

        private Vector2 GetSpawnPositionOutsidePlayspace()
        {
            float top = topWall ? topWall.position.y : 5f;
            float bottom = bottomWall ? bottomWall.position.y : -5f;
            float left = leftWall ? leftWall.position.x : -8f;
            float right = rightWall ? rightWall.position.x : 8f;

            return Random.Range(0, 4) switch
            {
                0 => new Vector2(Random.Range(left, right), top + spawnOffset),    // Top
                1 => new Vector2(Random.Range(left, right), bottom - spawnOffset), // Bottom
                2 => new Vector2(left - spawnOffset, Random.Range(bottom, top)),   // Left
                3 => new Vector2(right + spawnOffset, Random.Range(bottom, top)),  // Right
                _ => Vector2.zero
            };
        }

        private Vector2 GetSpawnTopPositionOutsidePlayspace()
        {
            float top = topWall ? topWall.position.y : 5f;
    
            float left = leftWall ? leftWall.position.x : -8f;
            float right = rightWall ? rightWall.position.x : 8f;

            float randomX = Random.Range(left, right);
            float spawnY = top + spawnOffset;

            return new Vector2(randomX, spawnY);
        }

        private Vector2 GetRandomDirectionTowardsPlayspace(Vector2 spawnPosition)
        {
            Vector2 targetArea = new(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
            return (targetArea - spawnPosition).normalized;
        }

        private float CalculateCurrentInterval()
        {
            int score = manager.Score;
            float baseInterval = Mathf.Max(_settings.minSpawnInterval, 
                _settings.startSpawnInterval - (score * _settings.spawnIntervalDecreasePerScore));
            
            float variance = Random.Range(-config.ballSpawnIntervalVariance, config.ballSpawnIntervalVariance);
            return Mathf.Max(0.1f, baseInterval + variance);
        }

        private void StopSpawning()
        {
            _spawnCts?.Cancel();
            _spawnCts?.Dispose();
            _spawnCts = null;
        }

        public void ClearAllBalls()
        {
            for (int i = _activeBalls.Count - 1; i >= 0; i--)
            {
                if (_activeBalls[i] != null) _ballPool.Release(_activeBalls[i]);
            }
            _activeBalls.Clear();
        }
        
        public void RemoveFromActiveList(BallController ball)
        {
            if(_activeBalls.Contains(ball))
                _activeBalls.Remove(ball);  
        } 
    }
}
