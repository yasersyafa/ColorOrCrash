using UnityEngine;

namespace ColorOfCrash.Features.Ball.Models
{
    [CreateAssetMenu(fileName = "BallSpawnerConfig", menuName = "Scriptable Objects/BallSpawnerConfig")]
    public class BallSpawnerConfig : ScriptableObject
    {
        public float spawnInterfal = 2f;
        public float minSpeed = 3f;
        public float maxSpeed = 6f;
        public GameObject ballPrefab;
    }
}
