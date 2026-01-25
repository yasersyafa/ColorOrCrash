using UnityEngine;
using ColorOrCrash.Vin.Core;

namespace ColorOrCrash.Vin.Player
{
    public class PlayerMovement
    {
        private Transform transform;
        private Rigidbody2D rb;
        private GameSettings settings;

        private Vector2 moveInput;
        private bool isDashing;
        private float dashTimer;
        private float dashCooldownTimer;
        private Vector2 dashDirection;

        public bool IsDashing => isDashing;

        public PlayerMovement(Transform transform, Rigidbody2D rigidbody, GameSettings settings)
        {
            this.transform = transform;
            this.rb = rigidbody;
            this.settings = settings;
        }

        public void SetMoveInput(Vector2 input)
        {
            moveInput = input;
        }

        public void TryDash()
        {
            if (dashCooldownTimer <= 0f && !isDashing && moveInput != Vector2.zero)
            {
                isDashing = true;
                dashTimer = settings.playerDashDuration;
                dashDirection = moveInput.normalized;
            }
        }

        public void FixedUpdate()
        {
            UpdateCooldowns();
            Move();
        }

        private void UpdateCooldowns()
        {
            if (dashCooldownTimer > 0f)
            {
                dashCooldownTimer -= Time.fixedDeltaTime;
            }

            if (isDashing)
            {
                dashTimer -= Time.fixedDeltaTime;
                if (dashTimer <= 0f)
                {
                    isDashing = false;
                    dashCooldownTimer = settings.playerDashCooldown;
                }
            }
        }

        private void Move()
        {
            Vector2 velocity;

            if (isDashing)
            {
                velocity = dashDirection * settings.playerDashSpeed;
            }
            else
            {
                velocity = moveInput * settings.playerMoveSpeed;
            }

            // Use velocity for physics-based movement (respects collisions)
            rb.linearVelocity = velocity;
        }

        public void Reset()
        {
            moveInput = Vector2.zero;
            isDashing = false;
            dashTimer = 0f;
            dashCooldownTimer = 0f;
            rb.linearVelocity = Vector2.zero;
        }
    }
}
