using System.Collections;
using ColorOrCrash.Global.Components;
using NocturneThree.ServiceLocator;
using UnityEngine;
using UnityEngine.Pool;

namespace ColorOrCrash.Features.Ball.Components
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
        private bool _isInsideArena = false;
        private Collider2D _collider;

        // width/height froom config
        private float widthArea, heightArea;

        public BallColor CurrentColor => _ballColor;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _renderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();

            _rb.linearDamping = 0;
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        public void Initialize(BallColor color, Vector2 position, Vector2 dir, float speed, IObjectPool<Ball> pool)
        {
            var manager = ServiceLocator.Get<GameManager>();
            if(manager != null)
            {
                widthArea = manager.config.targetAreaWidth;
                heightArea = manager.config.targetAreaHeight;
            }

            _pool = pool;
            _ballColor = color;
            transform.position = position;
            _speed = speed;
            _isInsideArena = false;

            _renderer.color = color switch
            {
                BallColor.Red => Color.red,
                BallColor.Blue => Color.blue,
                BallColor.Yellow => Color.yellow,
                BallColor.Green => Color.green,
                _ => Color.white
            };

            gameObject.SetActive(true);
            _rb.linearVelocity = dir * _speed;
        }

        void Update()
        {
            float currentVelocity = _rb.linearVelocity.magnitude;
            transform.Rotate(currentVelocity * rotationSpeedMultiplier * Time.deltaTime * Vector3.forward);

            if (!_isInsideArena)
            {
                // Cek apakah posisi bola sudah berada di dalam batas targetArea
                // Kamu bisa sesuaikan angka ini dengan ukuran targetAreaWidth/Height di config
                if (Mathf.Abs(transform.position.x) < widthArea && Mathf.Abs(transform.position.y) < heightArea)
                {
                    _isInsideArena = true;
                }
            }
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
