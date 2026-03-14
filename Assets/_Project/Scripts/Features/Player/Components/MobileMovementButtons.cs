using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ColorOrCrash.Features.Player.Components
{
    public class MobileMovementButtons : MonoBehaviour
    {
        // Assign di Inspector: Left Button & Right Button
        [SerializeField] private MobileButton leftButton;
        [SerializeField] private MobileButton rightButton;

        public Vector2 Input { get; private set; }

        private void Awake()
        {
            leftButton.OnHeld  += () => Input = Vector2.left;
            leftButton.OnReleased += UpdateInput;

            rightButton.OnHeld += () => Input = Vector2.right;
            rightButton.OnReleased += UpdateInput;
        }

        private void UpdateInput()
        {
            // Cek apakah salah satu masih ditekan
            if (leftButton.IsHeld) Input = Vector2.left;
            else if (rightButton.IsHeld) Input = Vector2.right;
            else Input = Vector2.zero;
        }

        private void OnDestroy()
        {
            leftButton.OnHeld -= () => Input = Vector2.left;
            leftButton.OnReleased -= UpdateInput;
            rightButton.OnHeld -= () => Input = Vector2.right;
            rightButton.OnReleased -= UpdateInput;
        }
    }
}