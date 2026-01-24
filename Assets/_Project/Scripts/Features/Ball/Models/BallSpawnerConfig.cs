using UnityEngine;

namespace ColorOrCrash.Features.Ball.Models
{
    [CreateAssetMenu(fileName = "BallSpawnerConfig", menuName = "Scriptable Objects/BallSpawnerConfig")]
    public class BallSpawnerConfig : ScriptableObject
    {
        public float spawnInterfal = 2f;
        public float minSpeed = 3f;
        public float maxSpeed = 6f;
        public GameObject ballPrefab;

        [Header("Radius Settings")]
        public float spawnRadius = 12f;
        public float targetAreaWidth = 10f;
        public float targetAreaHeight = 6f;
    }
}
