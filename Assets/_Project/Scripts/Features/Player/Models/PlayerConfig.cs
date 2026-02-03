using UnityEngine;

namespace ColorOrCrash.Features.Player.Models
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Scriptable Objects/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Movement")]
        public float moveSpeed = 8f;
        public float acceleration = 50f;
        public float decceleration = 50f;

        [Header("Jump")]
        public float jumpForce = 12f;
        public float doubleJumpMultiplier = 0.8f;
        public float fallMultiplier = 3f;      // Untuk rasa jatuh yang lebih mantap
        public float lowJumpMultiplier = 2.5f; // Untuk variable jump (hold vs tap)
        
        [Header("Detection")]
        public Vector2 groundCheckSize = new(0.8f, 0.1f);
        public LayerMask groundLayer;
    
    }
}
