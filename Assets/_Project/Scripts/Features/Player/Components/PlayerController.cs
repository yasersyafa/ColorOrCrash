using System;
using System.Threading;
using ColorOfCrash.Utils;
using Cysharp.Threading.Tasks;
using GabrielBigardi.SpriteAnimator;
using NocturneThree.EventSystem;
using NocturneThree.ServiceLocator;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ColorOrCrash.Features.Player.Components
{
    using Models;
    using Achievement.Events;
    using Ball.Components;
    using Camera.Components;
    using Global.Components;
    using Vin.Core;

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerConfig config;
        [SerializeField] private SpriteAnimator animator;
        [SerializeField] private SpriteRenderer bodyRenderer;

        [Header("Mobile Input")]
        [SerializeField] private MobileMovementButtons mobileMovementButtons;
        [SerializeField] private MobileButton mobileJumpButton;
        [SerializeField] private GameObject mobileControlsUI;
        [SerializeField] private bool forceMobileInEditor = false;

#region Animation string
        private const string ANIM_IDLE = "Idle";
        private const string ANIM_MOVE = "Move";
        private const string ANIM_JUMP = "Jump";
        private const string ANIM_FALL = "Fall";
        private const string ANIM_LAND = "Landing";
        private const string ANIM_DEATH = "Death";
#endregion

        private Rigidbody2D _rb;
        private CancellationTokenSource _colorCts;
        private bool _isMobile;

#region Color Settings
        private GameColor _currentType;
        private GameColor _nextType;
        private float _colorTimer;
        private const float COLOR_DURATION = 10f;
#endregion

#region Movement Settings
        private Vector2 _moveInput;
        private Vector3 _initialPosition;
        private bool _isGrounded;
        private bool _canDoubleJump;
        private bool _isJumpPressed;
        private bool _jumpRequest;
        private bool _isDoubleJumping;
        private bool _isLanding;
        private bool _isDead = false;
        private string _currentAnimation;
#endregion

#region Services
        private GameManager manager;
        private CameraShakeController cameraController;
#endregion

        public event Action<GameColor, GameColor, float, float> OnColorChanged;

        public GameColor CurrentType => _currentType;
        public GameColor NextType => _nextType;

        public bool IsMobileDevice()
        {
            #if UNITY_EDITOR
                return forceMobileInEditor;
            #elif UNITY_WEBGL
                return MobileDetector.IsMobile();
            #elif UNITY_ANDROID || UNITY_IOS
                return true;
            #else
                return false;
            #endif
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _colorCts = new CancellationTokenSource();
        }

        private void Start()
        {
            manager = FindAnyObjectByType<GameManager>();
            if (manager == null)
            {
                enabled = false;
                return;
            }

            cameraController = ServiceLocator.Get<CameraShakeController>();

            _initialPosition = transform.position;
            _isMobile = IsMobileDevice();

            manager.OnGameStateChanged += HandleGameStateChanged;

            SetupMobileControls();
            ResetPlayer();
            ColorSwapLoop(_colorCts.Token).Forget();
        }

        private void SetupMobileControls()
        {
            if (_isMobile && mobileControlsUI != null)
                mobileControlsUI.SetActive(true);

            if (_isMobile && mobileJumpButton != null)
            {
                mobileJumpButton.OnHeld     += HandleMobileJumpPressed;
                mobileJumpButton.OnReleased += HandleMobileJumpReleased;
            }
        }

        private void HandleMobileJumpPressed()
        {
            _isJumpPressed = true;
            _jumpRequest = true;
        }

        private void HandleMobileJumpReleased()
        {
            _isJumpPressed = false;
        }

        private void HandleGameStateChanged(Global.Components.GameState state)
        {
            if (state == Global.Components.GameState.Playing)
                ResetPlayer();
        }

        #region Input System Callbacks

        public void OnMove(InputAction.CallbackContext value)
        {
            if (_isMobile) return; // Mobile pakai button, ignore keyboard
            _moveInput = value.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext value)
        {
            if (_isMobile) return; // Mobile pakai button, ignore keyboard
            _isJumpPressed = value.performed;

            if (_isJumpPressed)
                _jumpRequest = true;
        }

        #endregion

        void Update()
        {
            if (_isDead || manager.CurrentState != Global.Components.GameState.Playing) return;

            if (_isMobile && mobileMovementButtons != null)
                _moveInput = mobileMovementButtons.Input;

            HandleAnimation();
            HandleSpriteFlip();
        }

        private void FixedUpdate()
        {
            if (manager.CurrentState != Global.Components.GameState.Playing)
            {
                _rb.linearVelocity = Vector2.zero;
                _rb.bodyType = RigidbodyType2D.Kinematic;
                return;
            }

            _rb.bodyType = RigidbodyType2D.Dynamic;

            CheckGround();
            ApplyMovement();
            ApplyJump();
            ApplyGravityModifiers();
        }

#region Custom Methods

        public void ResetPlayer()
        {
            _isDead = false;
            _rb.simulated = true;
            _rb.linearVelocity = Vector2.zero;

            transform.position = _initialPosition;

            _currentType = EnumUtils.GetRandomEnumValue<GameColor>();
            _nextType = GetUniqueRandomColor(_currentType);
            UpdateVisual(_currentType);

            _colorTimer = COLOR_DURATION;
            OnColorChanged?.Invoke(_currentType, _nextType, _colorTimer, COLOR_DURATION);

            ChangeAnimation(ANIM_IDLE);
        }

        public float GetColorTimerNormalized() => _colorTimer / COLOR_DURATION;

        private void HandleAnimation()
        {
            if (!_isGrounded)
            {
                if (_rb.linearVelocity.y > 0)
                {
                    ChangeAnimation(ANIM_JUMP);
                }
                else
                {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, config.groundLayer);
                    ChangeAnimation(hit.collider != null ? ANIM_LAND : ANIM_FALL);
                }
                return;
            }

            ChangeAnimation(Mathf.Abs(_rb.linearVelocity.x) > 0.1f ? ANIM_MOVE : ANIM_IDLE);
        }

        private void ChangeAnimation(string animName, Action onComplete = null)
        {
            if (_currentAnimation == animName) return;
            animator.Play(animName).SetOnComplete(onComplete);
            _currentAnimation = animName;
        }

        private void HandleSpriteFlip()
        {
            if (Mathf.Abs(_moveInput.x) > 0.01f)
                bodyRenderer.flipX = _moveInput.x < 0;
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
                    EventBus.Publish(new ProgressUpdateEvent(Achievement.Models.MissionType.Jump, 1));
                }
                else if (_canDoubleJump)
                {
                    _isDoubleJumping = true;
                    ExecuteJump(config.doubleJumpMultiplier);
                    _canDoubleJump = false;
                    EventBus.Publish(new ProgressUpdateEvent(Achievement.Models.MissionType.DoubleJump, 1));
                }
                _jumpRequest = false;
            }
        }

        private void ExecuteJump(float force)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
            _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            ServiceLocator.Get<AudioManager>().PlaySFX("Jump");
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
                OnColorChanged?.Invoke(_currentType, _nextType, _colorTimer, COLOR_DURATION);
                _colorTimer = COLOR_DURATION;

                while (_colorTimer > 0)
                {
                    if (manager.CurrentState == Global.Components.GameState.Playing && !_isDead)
                        _colorTimer -= Time.deltaTime;

                    await UniTask.Yield(token);
                }

                _currentType = _nextType;
                _nextType = GetUniqueRandomColor(_currentType);
                UpdateVisual(_currentType);
            }
        }

        private GameColor GetUniqueRandomColor(GameColor excludeColor)
        {
            GameColor newColor;
            do
            {
                newColor = EnumUtils.GetRandomEnumValue<GameColor>();
            }
            while (newColor == excludeColor);

            return newColor;
        }

        private void UpdateVisual(GameColor color)
        {
            if (bodyRenderer != null)
                bodyRenderer.color = color switch
                {
                    GameColor.Red   => manager.settings.redColor,
                    GameColor.Blue  => manager.settings.blueColor,
                    GameColor.Green => manager.settings.greenColor,
                    _               => Color.white
                };
        }

#endregion

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                if (collision.gameObject.TryGetComponent<BallController>(out var ball))
                {
                    if (ball.BallColor == _currentType)
                    {
                        cameraController?.Shake();
                        ServiceLocator.Get<AudioManager>().PlaySFX("Score");
                        manager.AddPoint(_currentType);
                        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, config.jumpForce * 0.5f);
                    }
                    else
                    {
                        if (manager.CurrentState != Global.Components.GameState.GameOver)
                        {
                            _rb.simulated = false;
                            _isDead = true;
                            ChangeAnimation(ANIM_DEATH, () => manager.ChangeState(Global.Components.GameState.GameOver));
                            ServiceLocator.Get<AudioManager>().PlaySFX("Death");
                        }
                    }
                    ball.OnCollected();
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

            if (mobileJumpButton != null)
            {
                mobileJumpButton.OnHeld     -= HandleMobileJumpPressed;
                mobileJumpButton.OnReleased -= HandleMobileJumpReleased;
            }

            if (manager != null)
                manager.OnGameStateChanged -= HandleGameStateChanged;
        }
    }
}