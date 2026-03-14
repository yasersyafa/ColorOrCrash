using UnityEngine;
using UnityEngine.UI;
using ColorOrCrash.Vin.Core;
using NocturneThree.ServiceLocator;
using ColorOrCrash.Features.Player.Components;
using ColorOrCrash.Global.Components;

namespace ColorOrCrash.Vin.UI
{
    public class ColorTimerBar : MonoBehaviour
    {
        [Header("Current Color Bar (Foreground)")]
        [SerializeField] private RectTransform barRect;
        [SerializeField] private Image barImage;
        
        [Header("Next Color Bar (Background)")]
        [SerializeField] private Image nextBarImage;

        [Header("Dependencies")]
        [SerializeField] private PlayerController _player;
        private GameManager _manager;

        private void Start()
        {
            _manager = FindAnyObjectByType<GameManager>();
            if (_manager == null)
            {
                enabled = false;
                return;
            }

            if (_player != null)
            {
                _player.OnColorChanged += HandleColorChanged;
                HandleColorChanged(_player.CurrentType, _player.NextType, 0, 0);
            }
        }

        private void OnDestroy()
        {
            if (_player != null)
            {
                _player.OnColorChanged -= HandleColorChanged;
            }
        }

        private void Update()
        {
            // Gunakan GameState dari Manager kamu
            if (_manager == null ||_manager.CurrentState != Global.Components.GameState.Playing) return;

            if (_player != null)
            {
                float t = Mathf.Clamp01(_player.GetColorTimerNormalized());
                UpdateBarWidth(t);
            }
        }

        private void HandleColorChanged(GameColor currentColor, GameColor nextColor, float timer, float maxTimer)
        {
            // Foreground Bar Color
            barImage.color = GetColorValue(currentColor);
            
            // Background Bar Color (Warna selanjutnya yang akan muncul)
            if (nextBarImage != null)
            {
                nextBarImage.color = GetColorValue(nextColor);
            }
        }

        private void UpdateBarWidth(float normalizedTime)
        {
            float parentWidth = ((RectTransform)barRect.parent).rect.width;
            float fullWidth = parentWidth - 40f; // minus padding 20 kiri + 20 kanan
            float currentWidth = fullWidth * normalizedTime;
            float shrink = (fullWidth - currentWidth) / 2f;

            barRect.offsetMin = new Vector2(20f + shrink, barRect.offsetMin.y);  // kiri
            barRect.offsetMax = new Vector2(-(20f + shrink), barRect.offsetMax.y);
        }

        private Color GetColorValue(GameColor color)
        {
            return color switch
            {
                GameColor.Red => _manager.settings.redColor,
                GameColor.Blue => _manager.settings.blueColor,
                GameColor.Green => _manager.settings.greenColor,
                _ => Color.white
            };
        }
    }
}
