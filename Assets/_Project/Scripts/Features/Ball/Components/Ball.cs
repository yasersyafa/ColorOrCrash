using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace ColorOfCrash.Features.Ball.Components
{
    /// <summary>
    /// Colour configuration for managing the colour of ball.
    /// You can add the colour whatever you want here.
    /// </summary>
    public enum BallColor
    {
        Red,
        Blue,
        Yellow,
        Green
    }

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    /// <summary>
    /// Representation for ball component
    /// </summary>
    public class Ball : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float rotationSpeedMultiplier = 50f;

        private BallColor _ballColor;
        private Rigidbody2D _rb;
        private float _speed;
        private Vector2 _direction;
        private SpriteRenderer _renderer;
        private IObjectPool<Ball> _pool;

        public BallColor CurrentColor => _ballColor;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _renderer = GetComponent<SpriteRenderer>();

            _rb.linearDamping = 0;
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        public void Initialize(BallColor color, Vector2 position, Vector2 dir, float speed, IObjectPool<Ball> pool)
        {
            _pool = pool;
            _ballColor = color;
            transform.position = position;
            _direction = dir;
            _speed = speed;

            switch(color)
            {
                case BallColor.Red:
                    _renderer.color = Color.red;
                    break;
                case BallColor.Blue:
                    _renderer.color = Color.blue;
                    break;
                case BallColor.Yellow:
                    _renderer.color = Color.yellow;
                    break;
                case BallColor.Green:
                    _renderer.color = Color.green;
                    break;
            }

            gameObject.SetActive(true);
            Vector2 throwDirection = new Vector2(dir.x, 0.5f).normalized;
            _rb.linearVelocity = throwDirection * _speed;
        }

        void Update()
        {
            float currentVelocity = _rb.linearVelocity.magnitude;
            transform.Rotate(Vector3.forward * currentVelocity * rotationSpeedMultiplier * Time.deltaTime);
        }

        void FixedUpdate()
        {
            if (_rb.linearVelocity.magnitude > 0)
            {
                _rb.linearVelocity = _rb.linearVelocity.normalized * _speed;
            }
        }

        private void OnCollisionEnter2D(Collision2D collider)
        {
            StopAllCoroutines();
            StartCoroutine(HitEffect());
        }

        private IEnumerator HitEffect()
        {
            transform.localScale = new Vector3(1.2f, 0.8f, 1f);
            float elapsed = 0;
            while (elapsed < 0.1f)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, elapsed / 0.1f);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.localScale = Vector3.one;
        }

        public void ReturnToPool()
        {
            _pool.Release(this);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
