using System;
using System.Threading;
using ColorOrCrash.Features.Player.Models;
using ColorOrCrash.Features.Tutorial.Components;
using ColorOrCrash.Global.Components;
using GabrielBigardi.SpriteAnimator;
using NocturneThree.ServiceLocator;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ColorOrCrash.Features.Player.Components
{
    public class PlayerTutorial : MonoBehaviour
    {
        [SerializeField] private PlayerConfig config;
        [SerializeField] private SpriteAnimator animator;
        [SerializeField] private SpriteRenderer bodyRenderer;
        [SerializeField] private TutorialManager _tutorialManager;

#region Animation string
        private const string ANIM_IDLE = "Idle";
        private const string ANIM_MOVE = "Move";
        private const string ANIM_JUMP = "Jump";
        private const string ANIM_FALL = "Fall";
        private const string ANIM_LAND = "Landing";
#endregion

        private Rigidbody2D _rb;

#region Movement Settings
        private Vector2 _moveInput;
        private bool _isGrounded;
        private bool _canDoubleJump;
        private bool _isJumpPressed;
        private bool _jumpRequest;
        private bool _isDoubleJumping;
        private string _currentAnimation;
#endregion

#region Tutorial Checker Properties
        public bool HasMovedLeft { get; private set; }
        public bool HasMovedRight { get; private set; }
        public bool HasJumped { get; private set; }
        public bool HasDoubleJumped { get; private set; }
        public float CurrentJumpHeight { get; private set; }
        public bool IsGrounded => _isGrounded;
#endregion

        public void OnMove(InputAction.CallbackContext value)
        {
            if (_tutorialManager != null && _tutorialManager.CurrentState != null)
            {
                if (!_tutorialManager.CurrentState.CanMove)
                {
                    _moveInput = Vector2.zero;
                    return;
                }
            }

            _moveInput = value.ReadValue<Vector2>();

            if (_moveInput.x > 0.1f) HasMovedRight = true;
            if (_moveInput.x < -0.1f) HasMovedLeft = true;
        }

        public void OnJump(InputAction.CallbackContext value)
        {
            if (_tutorialManager != null && _tutorialManager.CurrentState != null)
            {
                if (!_tutorialManager.CurrentState.CanMove) return;
            }

            _isJumpPressed = value.performed;
            
            if (_isJumpPressed)
            {
                _jumpRequest = true;
            }
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {
            HandleAnimation();
            HandleSpriteFlip();

            if (!_isGrounded)
            {
                CurrentJumpHeight = Mathf.Max(CurrentJumpHeight, transform.position.y);
            }
        }

        void FixedUpdate()
        {
            CheckGround();
            ApplyMovement();
            ApplyJump();
            ApplyGravityModifiers();
        }

#region Helper Methods

        private void HandleAnimation()
        {
            // 1. Air Logic (Jump, Fall, Landing Detection)
            if (!_isGrounded)
            {
                if (_rb.linearVelocity.y > 0)
                {
                    ChangeAnimation(ANIM_JUMP);
                }
                else
                {
                    // Landing Detection: Cek jarak ke tanah saat jatuh
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, config.groundLayer);
                    
                    if (hit.collider != null)
                        ChangeAnimation(ANIM_LAND);
                    else
                        ChangeAnimation(ANIM_FALL);
                }
                return;
            }

            // 2. Ground Logic (Idle, Move)
            if (Mathf.Abs(_rb.linearVelocity.x) > 0.1f)
            {
                ChangeAnimation(ANIM_MOVE);
            }
            else
            {
                ChangeAnimation(ANIM_IDLE);
            }
        }

        private void ChangeAnimation(string animName, Action onComplete = null)
        {
            // Prevent flickering: Hanya panggil Play jika nama animasi berbeda
            if (_currentAnimation == animName) return;

            animator.Play(animName).SetOnComplete(onComplete);
            _currentAnimation = animName;
        }

        private void HandleSpriteFlip()
        {
            if (Mathf.Abs(_moveInput.x) > 0.01f)
            {
                bodyRenderer.flipX = _moveInput.x < 0;
            }
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
                    HasJumped = true;
                }
                else if (_canDoubleJump)
                {
                    _isDoubleJumping = true;
                    ExecuteJump(config.doubleJumpMultiplier);
                    _canDoubleJump = false;
                    HasDoubleJumped = true;
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

            if (_isGrounded)
            {
                CurrentJumpHeight = 0;
            }
        }

        public void ResetTutorialFlags()
        {
            HasMovedLeft = false;
            HasMovedRight = false;
            HasJumped = false;
            HasDoubleJumped = false;
        }
        
#endregion
    }
}
