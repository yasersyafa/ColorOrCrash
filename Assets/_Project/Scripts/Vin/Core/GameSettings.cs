using UnityEngine;

namespace ColorOrCrash.Vin.Core
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "ColorOrCrash/Vin/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Colors")]
        public Color redColor = new Color(1f, 0.2f, 0.2f);
        public Color greenColor = new Color(0.2f, 1f, 0.2f);
        public Color blueColor = new Color(0.2f, 0.4f, 1f);
        public Color whiteColor = Color.white;

        [Header("Player Settings")]
        public float playerMoveSpeed = 5f;
        public float playerDashSpeed = 15f;
        public float playerDashDuration = 0.2f;
        public float playerDashCooldown = 1f;
        [Tooltip("Starting interval for color change (slower)")]
        public float startColorChangeInterval = 5f;
        [Tooltip("Minimum interval for color change (fastest)")]
        public float minColorChangeInterval = 1.5f;
        [Tooltip("How much interval decreases per score point")]
        public float colorChangeIntervalDecreasePerScore = 0.05f;

        [Header("Ball Settings")]
        public float ballMinSpeed = 2f;
        public float ballMaxSpeed = 5f;
        public float ballSpawnInterval = 2f;
        public float ballSpawnIntervalVariance = 1f;
        public float ballColorTransitionDuration = 1f;
        public float ballInvulnerableDuration = 1.5f;
        public int maxBallsInScene = 10;
        [Range(0.3f, 0.9f)] public float ballSpawnScale = 0.5f;

        [Header("Difficulty Progression")]
        [Tooltip("Spawn interval at start (slower)")]
        public float startSpawnInterval = 3f;
        [Tooltip("Minimum spawn interval (fastest)")]
        public float minSpawnInterval = 0.5f;
        [Tooltip("How much interval decreases per score point")]
        public float spawnIntervalDecreasePerScore = 0.05f;
        
        [Tooltip("Chance to spawn matching color at start (0-1)")]
        [Range(0f, 1f)] public float startMatchingColorChance = 0.5f;
        [Tooltip("Minimum chance to spawn matching color (0-1)")]
        [Range(0f, 1f)] public float minMatchingColorChance = 0.2f;
        [Tooltip("How much matching chance decreases per score point")]
        public float matchingChanceDecreasePerScore = 0.01f;

        public Color GetColor(GameColor gameColor)
        {
            return gameColor switch
            {
                GameColor.Red => redColor,
                GameColor.Green => greenColor,
                GameColor.Blue => blueColor,
                _ => whiteColor
            };
        }
    }
}
