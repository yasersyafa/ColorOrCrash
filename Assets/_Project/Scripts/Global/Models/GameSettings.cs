using ColorOrCrash.Vin.Core;
using UnityEngine;

namespace ColorOrCrash.Global.Models
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Colors")]
        public Color redColor = new(1f, 0.2f, 0.2f);
        public Color greenColor = new(0.2f, 1f, 0.2f);
        public Color blueColor = new(0.2f, 0.4f, 1f);
        public Color whiteColor = Color.white;

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
