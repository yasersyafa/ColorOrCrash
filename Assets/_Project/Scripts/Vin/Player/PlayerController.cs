using System;
using UnityEngine;
using ColorOrCrash.Vin.Core;
using ColorOrCrash.Vin.Managers;
using ColorOrCrash.Vin.Ball;

namespace ColorOrCrash.Vin.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D playerCollider;
        [SerializeField] private Rigidbody2D rb;

        private GameSettings settings;
        private PlayerColorManager colorManager;
        private PlayerMovement movement;

        public GameColor CurrentColor => colorManager.CurrentColor;
        public event Action<GameColor, GameColor, float, float> OnColorChanged; // current, next, timer, maxTimer

        private void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            
            settings = GameManager.Instance.Settings;
            colorManager = new PlayerColorManager(settings);
            movement = new PlayerMovement(transform, rb, settings);
        }

        private void Start()
        {
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            colorManager.OnColorChanged += HandleColorChanged;
            colorManager.Initialize();
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
            }
            colorManager.OnColorChanged -= HandleColorChanged;
        }

        private void Update()
        {
            if (GameManager.Instance.CurrentState != GameState.Playing) return;

            HandleInput();
            colorManager.Update(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (GameManager.Instance.CurrentState != GameState.Playing) return;
            movement.FixedUpdate();
        }

        private void HandleInput()
        {
            // Movement input
            Vector2 input = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;

            movement.SetMoveInput(input);

            // Dash input
            if (Input.GetKeyDown(KeyCode.J))
            {
                movement.TryDash();
            }
        }

        private void HandleColorChanged(GameColor currentColor, GameColor nextColor, float timer, float maxTimer)
        {
            spriteRenderer.color = settings.GetColor(currentColor);
            OnColorChanged?.Invoke(currentColor, nextColor, timer, maxTimer);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (GameManager.Instance.CurrentState != GameState.Playing) return;

            if (collision.gameObject.TryGetComponent<BallController>(out var ball))
            {
                if (!ball.IsActive) return; // Ball masih invulnerable

                if (ball.BallColor == CurrentColor)
                {
                    // Same color - score!
                    GameManager.Instance.AddScore();
                    ball.OnCollected();
                }
                else
                {
                    // Different color - game over!
                    GameManager.Instance.GameOver();
                }
            }
        }

        private void OnGameStateChanged(GameState newState)
        {
            if (newState == GameState.Playing)
            {
                ResetPlayer();
            }
        }

        private void ResetPlayer()
        {
            transform.position = Vector3.zero;
            colorManager.Initialize();
            movement.Reset();
        }

        public float GetColorTimerNormalized()
        {
            return colorManager.GetTimerNormalized();
        }
    }
}
