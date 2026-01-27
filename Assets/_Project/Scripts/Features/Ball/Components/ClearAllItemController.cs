using ColorOrCrash.Features.Ball.Models;
using ColorOrCrash.Features.Camera.Components;
using ColorOrCrash.Features.Player.Components;
using ColorOrCrash.Global.Components;
using Cysharp.Threading.Tasks;
using NocturneThree.ServiceLocator;
using UnityEngine;

namespace ColorOrCrash.Features.Ball.Components
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    /// <summary>
    /// Item that clears all balls when collected. Behaves similarly to balls but only one active in scene.
    /// </summary>
    public class ClearAllItemController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private CircleCollider2D _collider;

        private BallSpawner _spawner;
        private BallSpawnerConfig _config;

        private Vector2 _direction;
        private float _speed;
        private bool _isActive = false;
        private float _originalScale;
        [SerializeField] private Color itemColor = Color.white;

        private void Awake()
        {
            if (!_renderer) _renderer = GetComponent<SpriteRenderer>();
            if (!_rb) _rb = GetComponent<Rigidbody2D>();
            if (!_collider) _collider = GetComponent<CircleCollider2D>();

            _originalScale = transform.localScale.x;

            _rb.gravityScale = 0f;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            _collider.isTrigger = true; // invulnerable until transition finishes
            _renderer.color = itemColor; // keep constant color (no ball-like color transition) 
        }

        public void Initialize(BallSpawner spawner, Vector2 direction, float speed, BallSpawnerConfig config)
        {
            _spawner = spawner;
            _direction = direction.normalized;
            _speed = speed;
            _config = config;

            transform.localScale = _originalScale * _config.itemSpawnScale * Vector3.one;
            _rb.linearVelocity = _direction * _speed;

            HandleLifecycleAsync().Forget();
        }

        private async UniTaskVoid HandleLifecycleAsync()
        {
            // 1. Invulnerable wait
            await UniTask.Delay(System.TimeSpan.FromSeconds(_config.itemInvulnerableDuration), cancellationToken: this.GetCancellationTokenOnDestroy());

            // 2. Transition visual (scale only) - keep color constant
            float elapsed = 0;
            float duration = _config.itemTransitionDuration;
            float startScale = _originalScale * _config.itemSpawnScale;

            while (elapsed < duration)
            {
                if (ServiceLocator.Get<GameManager>().CurrentState != GameState.Playing)
                {
                    await UniTask.Yield();
                    continue;
                }

                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float smoothT = Mathf.SmoothStep(0f, 1f, t);

                // Keep the item color constant
                transform.localScale = Vector3.one * Mathf.Lerp(startScale, _originalScale, smoothT);

                await UniTask.Yield();
            }

            // Finalize activation
            _renderer.color = itemColor;
            transform.localScale = Vector3.one * _originalScale;
            _isActive = true;
            _collider.isTrigger = false;
            _rb.linearVelocity = _direction * _speed; // ensure physics continues with correct velocity
        }

        private void Update()
        {
            // Boundary check
            if (_isActive && transform.position.sqrMagnitude > 2500f)
            {
                // Notify spawner and destroy
                _spawner?.NotifyItemCollected(this);
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!_isActive) return;

            if (collision.TryGetComponent<PlayerController>(out var player))
            {
                OnCollected();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                // Reflect direction and update velocity to keep bounce behavior
                _direction = Vector2.Reflect(_direction, collision.contacts[0].normal).normalized;
                _rb.linearVelocity = _direction * _speed;
            }
            else if (collision.collider != null && collision.collider.TryGetComponent<PlayerController>(out var player))
            {
                // Collect immediately on collision with player (prevents weird bounce-off effect)
                if (_isActive) OnCollected();
            }
        }

        public void OnCollected()
        {
            if (!_isActive) return;

            // Award score
            try
            {
                ServiceLocator.Get<GameManager>()?.AddScore(_config.itemScoreReward);
            }
            catch
            {
                // fallback in case service locator not available
            }

            // Camera shake for feedback
            ServiceLocator.Get<CameraShakeController>()?.Shake();

            // Clear all balls via spawner
            _spawner?.ClearAllBalls();
            _spawner?.NotifyItemCollected(this);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            // Ensure spawner knows item is gone
            _spawner?.NotifyItemCollected(this);
        }
    }
}