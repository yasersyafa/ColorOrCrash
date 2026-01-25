using UnityEngine;
using UnityEngine.UI;
using ColorOrCrash.Vin.Core;
using ColorOrCrash.Vin.Managers;
using ColorOrCrash.Vin.Player;

namespace ColorOrCrash.Vin.UI
{
    public class ColorTimerBar : MonoBehaviour
    {
        [Header("Current Color Bar (Foreground)")]
        [SerializeField] private RectTransform barRect;
        [SerializeField] private Image barImage;
        
        [Header("Next Color Bar (Background)")]
        [SerializeField] private Image nextBarImage;
        
        [Header("Settings")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private float maxWidth = 1920f;

        private GameSettings settings;

        private void Start()
        {
            settings = GameManager.Instance.Settings;

            if (playerController != null)
            {
                playerController.OnColorChanged += OnColorChanged;
            }
        }

        private void OnDestroy()
        {
            if (playerController != null)
            {
                playerController.OnColorChanged -= OnColorChanged;
            }
        }

        private void Update()
        {
            if (GameManager.Instance.CurrentState != GameState.Playing) return;

            if (playerController != null)
            {
                UpdateBarWidth(playerController.GetColorTimerNormalized());
            }
        }

        private void OnColorChanged(GameColor currentColor, GameColor nextColor, float timer, float maxTimer)
        {
            // Set current color bar (foreground)
            barImage.color = settings.GetColor(currentColor);
            
            // Set next color bar (background) - this will be revealed as current bar shrinks
            if (nextBarImage != null)
            {
                nextBarImage.color = settings.GetColor(nextColor);
            }

            // Reset bar width
            UpdateBarWidth(1f);
        }

        private void UpdateBarWidth(float normalizedTime)
        {
            float currentWidth = maxWidth * normalizedTime;

            Vector2 size = barRect.sizeDelta;
            size.x = currentWidth;
            barRect.sizeDelta = size;
        }
    }
}
