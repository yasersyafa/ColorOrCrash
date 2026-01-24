using System.Threading;
using ColorOfCrash.Utils;
using ColorOrCrash.Features.Ball.Models;
using Cysharp.Threading.Tasks;
using NocturneThree.ServiceLocator;
using UnityEngine;
using UnityEngine.Pool;

namespace ColorOrCrash.Features.Ball.Components
{
    public class BallSpawner : MonoBehaviour, IGameService
    {
        public BallSpawnerConfig config;
        [SerializeField] private Transform[] spawnPoints;

        private IObjectPool<Ball> _pool;
        private float _timer;
        private CancellationTokenSource _cts;

        private void Awake()
        {
            // Inisialisasi Object Pool
            _pool = new ObjectPool<Ball>(
                createFunc: () => Instantiate(config.ballPrefab).GetComponent<Ball>(),
                actionOnGet: (ball) => { /* Reset logic if needed */ },
                actionOnRelease: (ball) => ball.Deactivate(),
                actionOnDestroy: (ball) => Destroy(ball.gameObject),
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 500
            );

            ServiceLocator.Register<BallSpawner>(this);
            _cts = new();
        }

        void Start()
        {
            StartSpawning(_cts.Token).Forget();
        }

        private async UniTask StartSpawning(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Spawn();

                await UniTask.Delay(System.TimeSpan.FromSeconds(config.spawnInterfal), cancellationToken: token);
            }
        }

        void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
        }
        
        public void Spawn()
        {
            if (spawnPoints.Length == 0) return;

            Transform selectedPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            
            float directionX = (selectedPoint.position.x < 0) ? 1f : -1f;
            Vector2 moveDir = new Vector2(directionX, 1);
            
            BallColor randomColor = EnumUtils.GetRandomEnumValue<BallColor>();

            Ball ball = _pool.Get();
            ball.Initialize(
                randomColor, 
                selectedPoint.position, 
                moveDir, 
                Random.Range(config.minSpeed, config.maxSpeed),
                _pool
            );
        }

        public void ReleaseBall(Ball ball)
        {
            _pool.Release(ball);
        }
    }
}
