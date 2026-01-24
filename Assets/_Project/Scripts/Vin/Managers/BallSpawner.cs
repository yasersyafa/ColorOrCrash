using System.Collections.Generic;
using UnityEngine;
using ColorOrCrash.Vin.Ball;
using ColorOrCrash.Vin.Core;
using ColorOrCrash.Vin.Player;

namespace ColorOrCrash.Vin.Managers
{
    public class BallSpawner : MonoBehaviour
    {
        [SerializeField] private BallController ballPrefab;
        [SerializeField] private Transform ballContainer;
        [SerializeField] private PlayerController playerController;
        
        [Header("Wall References")]
        [SerializeField] private Transform topWall;
        [SerializeField] private Transform bottomWall;
        [SerializeField] private Transform leftWall;
        [SerializeField] private Transform rightWall;
        [SerializeField] private float spawnOffset = 1.5f;

        private GameSettings settings;
        private float spawnTimer;
        private List<BallController> activeBalls = new List<BallController>();

        private void Start()
        {
            settings = GameManager.Instance.Settings;
            ResetSpawnTimer();
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
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
            if (GameManager.Instance.CurrentState != GameState.Playing) return;

            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0f && activeBalls.Count < settings.maxBallsInScene)
            {
                SpawnBall();
                ResetSpawnTimer();
            }

            // Clean up destroyed balls
            activeBalls.RemoveAll(b => b == null);
        }

        private void SpawnBall()
        {
            Vector2 spawnPosition = GetSpawnPositionOutsidePlayspace();
            Vector2 direction = GetRandomDirectionTowardsPlayspace(spawnPosition);

            BallController ball = Instantiate(ballPrefab, spawnPosition, Quaternion.identity, ballContainer);
            
            GameColor ballColor = GetBiasedRandomColor();
            float speed = Random.Range(settings.ballMinSpeed, settings.ballMaxSpeed);

            ball.Initialize(ballColor, direction, speed);
            activeBalls.Add(ball);
        }

        private GameColor GetBiasedRandomColor()
        {
            int currentScore = GameManager.Instance.Score;
            
            // Calculate matching color chance - decreases per score point
            float matchingChance = settings.startMatchingColorChance - (currentScore * settings.matchingChanceDecreasePerScore);
            matchingChance = Mathf.Max(matchingChance, settings.minMatchingColorChance);
            
            // Get player's current color
            GameColor playerColor = playerController != null ? playerController.CurrentColor : (GameColor)Random.Range(0, 3);
            
            float roll = Random.value;
            
            if (roll < matchingChance)
            {
                // Spawn matching color
                return playerColor;
            }
            else
            {
                // Spawn one of the other colors (equal chance)
                GameColor otherColor;
                do
                {
                    otherColor = (GameColor)Random.Range(0, 3);
                } while (otherColor == playerColor);
                return otherColor;
            }
        }

        private Vector2 GetSpawnPositionOutsidePlayspace()
        {
            float top = topWall != null ? topWall.position.y : 5f;
            float bottom = bottomWall != null ? bottomWall.position.y : -5f;
            float left = leftWall != null ? leftWall.position.x : -8f;
            float right = rightWall != null ? rightWall.position.x : 8f;

            int side = Random.Range(0, 4);

            return side switch
            {
                0 => new Vector2(Random.Range(left, right), top + spawnOffset), // Top
                1 => new Vector2(Random.Range(left, right), bottom - spawnOffset), // Bottom
                2 => new Vector2(left - spawnOffset, Random.Range(bottom, top)), // Left
                3 => new Vector2(right + spawnOffset, Random.Range(bottom, top)), // Right
                _ => Vector2.zero
            };
        }

        private Vector2 GetRandomDirectionTowardsPlayspace(Vector2 spawnPosition)
        {
            // Arah menuju area tengah playspace dengan sedikit variasi
            Vector2 centerOffset = new Vector2(
                Random.Range(-2f, 2f),
                Random.Range(-2f, 2f)
            );

            return (centerOffset - spawnPosition).normalized;
        }

        private void ResetSpawnTimer()
        {
            int currentScore = GameManager.Instance.Score;
            
            // Calculate spawn interval - decreases per score point
            float baseInterval = settings.startSpawnInterval - (currentScore * settings.spawnIntervalDecreasePerScore);
            baseInterval = Mathf.Max(baseInterval, settings.minSpawnInterval);
            
            float variance = Random.Range(-settings.ballSpawnIntervalVariance, settings.ballSpawnIntervalVariance);
            spawnTimer = Mathf.Max(0.1f, baseInterval + variance);
        }

        public void ClearAllBalls()
        {
            foreach (var ball in activeBalls)
            {
                if (ball != null)
                {
                    Destroy(ball.gameObject);
                }
            }
            activeBalls.Clear();
        }

        private void OnGameStateChanged(GameState newState)
        {
            if (newState == GameState.Menu || newState == GameState.GameOver)
            {
                // Optionally clear balls when game ends
            }
            else if (newState == GameState.Playing)
            {
                ClearAllBalls();
                ResetSpawnTimer();
            }
        }
    }
}
