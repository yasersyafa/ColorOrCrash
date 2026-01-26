using UnityEngine;

namespace ColorOrCrash.Features.Ball.Models
{
    [CreateAssetMenu(fileName = "BallSpawnerConfig", menuName = "Scriptable Objects/BallSpawnerConfig")]
    public class BallSpawnerConfig : ScriptableObject
    {
        public GameObject ballPrefab;

        [Header("Ball Settings")]
        public float ballMinSpeed = 2f;
        public float ballMaxSpeed = 5f;
        public float ballSpawnInterval = 2f;
        public float ballSpawnIntervalVariance = 1f;
        public float ballColorTransitionDuration = 1f;
        public float ballInvulnerableDuration = 1.5f;
        public int maxBallsInScene = 10;
        [Range(0.3f, 0.9f)] public float ballSpawnScale = 0.5f;
    }
}
