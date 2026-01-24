using UnityEngine;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;
using System.Threading;
using ColorOrCrash.Features.Ball.Components;
using ColorOrCrash.Features.Ball.Models;
using NocturneThree.ServiceLocator;

namespace ColorOrCrash.Global.Components
{
    [Service]
    public class GameManager : MonoBehaviour, IGameService
    {
        public BallSpawnerConfig config;
        
        private IObjectPool<Ball> _ballPool;
        private CancellationTokenSource _cts;

        private void Awake()
        {
            _cts = new CancellationTokenSource();
            
            // Inisialisasi Pool
            _ballPool = new ObjectPool<Ball>(
                createFunc: CreateBall,
                actionOnGet: OnGetFromPool,
                actionOnRelease: OnReleaseToPool,
                actionOnDestroy: OnDestroyObject,
                collectionCheck: false,
                defaultCapacity: 15,
                maxSize: 30
            );
        }

        private void Start()
        {
            StartSpawning(_cts.Token).Forget();
        }

        private async UniTaskVoid StartSpawning(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                SpawnBall();
                
                await UniTask.Delay(System.TimeSpan.FromSeconds(config.spawnInterfal), cancellationToken: token);
            }
        }

        private void SpawnBall()
        {
            float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector2 spawnPos = new Vector2(
                Mathf.Cos(randomAngle) * config.spawnRadius,
                Mathf.Sin(randomAngle) * config.spawnRadius
            );

            Vector2 targetPos = new Vector2(
                Random.Range(-config.targetAreaWidth / 2f, config.targetAreaWidth / 2f),
                Random.Range(-config.targetAreaHeight / 2f, config.targetAreaHeight / 2f)
            );

            Vector2 direction = (targetPos - spawnPos).normalized;
            float speed = Random.Range(config.minSpeed, config.maxSpeed);

            BallColor randomColor = (BallColor)Random.Range(0, System.Enum.GetValues(typeof(BallColor)).Length);

            Ball ball = _ballPool.Get();
            ball.Initialize(randomColor, spawnPos, direction, speed, _ballPool);
        }

        private Ball CreateBall() => Instantiate(config.ballPrefab).GetComponent<Ball>();
        private void OnGetFromPool(Ball ball) => ball.gameObject.SetActive(true);
        private void OnReleaseToPool(Ball ball) => ball.Deactivate();
        private void OnDestroyObject(Ball ball) => Destroy(ball.gameObject);

        private void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (config == null) return;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, config.spawnRadius);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, new Vector3(config.targetAreaWidth, config.targetAreaHeight, 0));
        }
    }
}