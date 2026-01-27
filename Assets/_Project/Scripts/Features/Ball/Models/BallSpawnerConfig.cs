using UnityEngine;

namespace ColorOrCrash.Features.Ball.Models
{
    [CreateAssetMenu(fileName = "BallSpawnerConfig", menuName = "Scriptable Objects/BallSpawnerConfig")]
    public class BallSpawnerConfig : ScriptableObject
    {
        public GameObject ballPrefab;
        public GameObject clearItemPrefab;

        [Header("Ball Settings")]
        public float ballMinSpeed = 2f;
        public float ballMaxSpeed = 5f;
        public float ballSpawnInterval = 2f;
        public float ballSpawnIntervalVariance = 1f;
        public float ballColorTransitionDuration = 1f;
        public float ballInvulnerableDuration = 1.5f;
        public int maxBallsInScene = 10;
        [Range(0.3f, 0.9f)] public float ballSpawnScale = 0.5f;

        [Header("Clear Item Settings")]
        public float itemMinSpeed = 1f;
        public float itemMaxSpeed = 3f;
        public float itemInvulnerableDuration = 1.5f;
        public float itemTransitionDuration = 1f;
        public int maxItemsInScene = 1;
        [Range(0.3f, 1.5f)] public float itemSpawnScale = 0.7f;
        public int itemMinScoreToSpawn = 5000;
        public int itemScoreThresholdFor30 = 10000;
        [Range(0f,1f)] public float itemSpawnChanceAt5k = 0.25f;
        [Range(0f,1f)] public float itemSpawnChanceAt10k = 0.3f;

        [Header("Clear Item Reward")]
        public int itemScoreReward = 1000; // Score awarded when player collects the clear-all item
    }
}
