using System.Collections;
using ColorOrCrash.Features.Ball.Models;
using ColorOrCrash.Global.Components;
using ColorOrCrash.Vin.Core;
using Cysharp.Threading.Tasks;
using NocturneThree.ServiceLocator;
using UnityEngine;
using UnityEngine.Pool;
using Global = ColorOrCrash.Global.Models;

namespace ColorOrCrash.Features.Ball.Components
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    /// <summary>
    /// Representation for ball component
    /// </summary>
    public class BallController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private CircleCollider2D _collider;

        [Header("Configuration")]
        [SerializeField] private BallSpawnerConfig _config;

        private IObjectPool<BallController> _pool;
        private GameColor _ballColor;
        private float _speed;
        private Vector2 _direction;
        private bool _isActive = false;
        private float _originalScale;

        public GameColor BallColor => _ballColor;
        public bool IsActive => _isActive;
        public Global.Models.GameSettings settings;

        private void Awake()
        {
            if (!_renderer) _renderer = GetComponent<SpriteRenderer>();
            if (!_rb) _rb = GetComponent<Rigidbody2D>();
            if (!_collider) _collider = GetComponent<CircleCollider2D>();
            
            _originalScale = transform.localScale.x;

            _rb.gravityScale = 0f;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        public void Initialize(GameColor color, Vector2 direction, float speed, IObjectPool<BallController> pool)
        {
            _ballColor = color;
            _direction = direction.normalized;
            _speed = speed;
            _pool = pool;

            settings = ServiceLocator.Get<GameManager>().settings;

            _collider.isTrigger = true;
            _renderer.color = Color.white;
            transform.localScale = _originalScale * _config.ballSpawnScale * Vector3.one;
            
            _rb.linearVelocity = _direction * _speed;

            HandleLifecycleAsync().Forget();
        }

        private async UniTaskVoid HandleLifecycleAsync()
        {
            // 1. Invulnerable Wait
            await UniTask.Delay(System.TimeSpan.FromSeconds(_config.ballInvulnerableDuration), 
                cancellationToken: this.GetCancellationTokenOnDestroy());

            // 2. Transition State (Color & Scale)
            float elapsed = 0;
            float duration = _config.ballColorTransitionDuration;
            float startScale = _originalScale * _config.ballSpawnScale;
            Color targetColor = settings.GetColor(_ballColor);

            while (elapsed < duration)
            {
                if (ServiceLocator.Get<GameManager>().CurrentState != Global.Components.GameState.Playing)
                {
                    await UniTask.Yield();
                    continue;
                }

                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float smoothT = Mathf.SmoothStep(0f, 1f, t);

                // Apply Visuals
                _renderer.color = Color.Lerp(settings.whiteColor, targetColor, t);
                transform.localScale = Vector3.one * Mathf.Lerp(startScale, _originalScale, smoothT);

                await UniTask.Yield();
            }

            // 3. Finalize Activation
            _renderer.color = targetColor;
            transform.localScale = Vector3.one * _originalScale;
            _isActive = true;
            _collider.isTrigger = false;
        }

        private void Update()
        {
            // Check boundary
            if (_isActive && transform.position.sqrMagnitude > 2500f) // 50^2 for performance
            {
                OnCollected();
            }
        }

        private void FixedUpdate()
        {
            if (ServiceLocator.Get<GameManager>().CurrentState != Global.Components.GameState.Playing)
            {
                _rb.linearVelocity = Vector2.zero;
                return;
            }

            // Maintain constant physics speed
            if (_isActive && _rb.linearVelocity.sqrMagnitude > 0.01f)
            {
                _rb.linearVelocity = _rb.linearVelocity.normalized * _speed;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                _direction = Vector2.Reflect(_direction, collision.contacts[0].normal).normalized;
                _rb.linearVelocity = _direction * _speed;
                
                StopAllCoroutines();
                StartCoroutine(HitEffect());
            }
        }

        public void OnCollected()
        {
            // Releasing back to pool instead of destroying
            ServiceLocator.Get<BallSpawner>().RemoveFromActiveList(this);
            _pool?.Release(this);
        }

        private IEnumerator HitEffect()
        {
            transform.localScale = new Vector3(1.5f, 0.8f, 1f);
            float elapsed = 0;
            while (elapsed < 0.2f)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, elapsed / 0.1f);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.localScale = Vector3.one;
        }

        private void OnDestroy()
        {
            _pool = null;
        }
    }
}
