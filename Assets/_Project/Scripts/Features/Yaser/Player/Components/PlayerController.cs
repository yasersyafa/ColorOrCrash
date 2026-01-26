using System.Threading;
using ColorOfCrash.Utils;
using ColorOrCrash.Features.Ball.Components;
using ColorOrCrash.Features.Player.Models;
using ColorOrCrash.Global.Components;
using ColorOrCrash.Vin.Core;
using Cysharp.Threading.Tasks;
using NocturneThree.ServiceLocator;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ColorOrCrash.Features.Player.Components
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerConfig config;
        [SerializeField] private SpriteRenderer bodyRenderer;

        private Rigidbody2D _rb;
        private GameColor _currentType;
        private CancellationTokenSource _colorCts;

        private Vector2 _moveInput;
        private bool _isGrounded;
        private bool _canDoubleJump;
        private bool _isJumpPressed;
        private bool _jumpRequest;
        private bool _isDoubleJumping;
        private GameManager manager;

        public GameColor CurrentType => _currentType;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _colorCts = new CancellationTokenSource();
        }

        private void Start()
        {
            manager = ServiceLocator.Get<GameManager>();

            _currentType = EnumUtils.GetRandomEnumValue<GameColor>();
            UpdateVisual(_currentType);

            ColorSwapLoop(_colorCts.Token).Forget();
        }

        #region Input System Callbacks

        public void OnMove(InputAction.CallbackContext value)
        {
            _moveInput = value.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext value)
        {
            _isJumpPressed = value.performed;
            
            if (_isJumpPressed)
            {
                _jumpRequest = true;
            }
        }
        #endregion

        private void FixedUpdate()
        {
            CheckGround();
            ApplyMovement();
            ApplyJump();
            ApplyGravityModifiers();
        }

        private void ApplyMovement()
        {
            float targetSpeed = _moveInput.x * config.moveSpeed;
            float speedDif = targetSpeed - _rb.linearVelocity.x;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? config.acceleration : config.decceleration;
            
            float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, 0.9f) * Mathf.Sign(speedDif);
            _rb.AddForce(movement * Vector2.right);
        }

        private void ApplyJump()
        {
            if (_jumpRequest)
            {
                if (_isGrounded)
                {
                    _isDoubleJumping = false;
                    ExecuteJump(config.jumpForce);
                    _canDoubleJump = true;
                }
                else if (_canDoubleJump)
                {
                    _isDoubleJumping = true;
                    ExecuteJump(config.doubleJumpMultiplier);
                    _canDoubleJump = false;
                }
                _jumpRequest = false;
            }
        }

        private void ExecuteJump(float force)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0); 
            _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        }

        private void ApplyGravityModifiers()
        {
            if (_rb.linearVelocity.y < 0)
            {
                _rb.linearVelocity += (config.fallMultiplier - 1) * Physics2D.gravity.y * Time.fixedDeltaTime * Vector2.up;
            }

            else if (_rb.linearVelocity.y > 0 && !_isDoubleJumping && !_isJumpPressed)
            {
                _rb.linearVelocity += (config.lowJumpMultiplier - 1) * Physics2D.gravity.y * Time.fixedDeltaTime * Vector2.up;
            }
        }

        private void CheckGround()
        {
            _isGrounded = Physics2D.OverlapBox((Vector2)transform.position + Vector2.down * 0.5f, config.groundCheckSize, 0, config.groundLayer);
        }

        private async UniTaskVoid ColorSwapLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(System.TimeSpan.FromSeconds(10f), cancellationToken: token);
                
                _currentType = EnumUtils.GetRandomEnumValue<GameColor>();
                UpdateVisual(_currentType);
            }
        }

        private void UpdateVisual(GameColor color)
        {
            if (bodyRenderer != null)
                bodyRenderer.color = color switch
                {
                    GameColor.Red => manager.settings.redColor,
                    GameColor.Blue => manager.settings.blueColor,
                    GameColor.Green => manager.settings.greenColor,
                    _ => Color.white
                };
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                if (collision.gameObject.TryGetComponent<BallController>(out var ball))
                {
                    if (ball.BallColor == _currentType)
                    {
                        Destroy(ball);
                        // TODO: add score and small bounce effect to player (optional )
                        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, config.jumpForce * 0.5f);
                    }
                    else
                    {
                        Debug.Log("Game Over - Wrong Color!");
                        // Trigger GameOver Event
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (_colorCts != null)
            {
                _colorCts.Cancel();
                _colorCts.Dispose();
            }
        }
    }
}
