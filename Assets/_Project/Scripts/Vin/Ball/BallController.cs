using UnityEngine;
using ColorOrCrash.Vin.Core;
using ColorOrCrash.Vin.Managers;

namespace ColorOrCrash.Vin.Ball
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public class BallController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private CircleCollider2D circleCollider;

        private GameSettings settings;
        private GameColor ballColor;
        private Vector2 direction;
        private float speed;

        private bool isActive = false;
        private float transitionTimer;
        private float invulnerableTimer;
        
        // Scale transition
        private float startScale;
        private float targetScale;
        private float originalScale;

        public GameColor BallColor => ballColor;
        public bool IsActive => isActive;

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (circleCollider == null) circleCollider = GetComponent<CircleCollider2D>();
            
            // Store original prefab scale
            originalScale = transform.localScale.x;

            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        public void Initialize(GameColor color, Vector2 direction, float speed)
        {
            settings = GameManager.Instance.Settings;
            this.ballColor = color;
            this.direction = direction.normalized;
            this.speed = speed;

            // Start as white and invulnerable
            spriteRenderer.color = settings.whiteColor;
            isActive = false;
            transitionTimer = 0f;
            invulnerableTimer = settings.ballInvulnerableDuration;

            // Start with smaller scale, target is original prefab scale
            targetScale = originalScale;
            startScale = originalScale * settings.ballSpawnScale;
            transform.localScale = Vector3.one * startScale;

            // Disable collision trigger while invulnerable
            circleCollider.isTrigger = true;

            rb.linearVelocity = this.direction * this.speed;
        }

        private void Update()
        {
            if (GameManager.Instance.CurrentState != GameState.Playing)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            if (!isActive)
            {
                HandleInvulnerableState();
            }
        }

        private void FixedUpdate()
        {
            // Maintain constant speed
            if (isActive && rb.linearVelocity.magnitude > 0.1f)
            {
                direction = rb.linearVelocity.normalized;
                rb.linearVelocity = direction * speed;
            }
        }

        private void HandleInvulnerableState()
        {
            invulnerableTimer -= Time.deltaTime;

            if (invulnerableTimer <= 0f)
            {
                // Start color and scale transition
                transitionTimer += Time.deltaTime;
                float t = Mathf.Clamp01(transitionTimer / settings.ballColorTransitionDuration);
                
                // Smooth easing for scale
                float smoothT = Mathf.SmoothStep(0f, 1f, t);

                // Color transition
                Color targetColor = settings.GetColor(ballColor);
                spriteRenderer.color = Color.Lerp(settings.whiteColor, targetColor, t);
                
                // Scale transition (from small to full size)
                float currentScale = Mathf.Lerp(startScale, targetScale, smoothT);
                transform.localScale = Vector3.one * currentScale;

                if (transitionTimer >= settings.ballColorTransitionDuration)
                {
                    spriteRenderer.color = targetColor;
                    transform.localScale = Vector3.one * targetScale;
                    isActive = true;
                    
                    // Enable physical collision for bouncing off walls
                    circleCollider.isTrigger = false;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Bounce off walls
            if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                Vector2 normal = collision.contacts[0].normal;
                direction = Vector2.Reflect(direction, normal).normalized;
                rb.linearVelocity = direction * speed;
            }
        }

        public void OnCollected()
        {
            // Play particle effect or animation here if needed
            Destroy(gameObject);
        }

        private void OnBecameInvisible()
        {
            // Destroy ball if it goes too far outside camera view
            if (isActive && transform.position.magnitude > 50f)
            {
                Destroy(gameObject);
            }
        }
    }
}
