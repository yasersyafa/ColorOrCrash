using UnityEngine;
using ColorOrCrash.Vin.Core;
using ColorOrCrash.Vin.Managers;
using ColorOrCrash.Vin.Player;

namespace ColorOrCrash.Vin.VFX
{
    public class PlayerDashTrail : MonoBehaviour
    {
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private PlayerController playerController;

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

        private void OnColorChanged(GameColor currentColor, GameColor nextColor, float timer, float maxTimer)
        {
            if (trailRenderer != null)
            {
                Color trailColor = settings.GetColor(currentColor);
                trailColor.a = 0.5f;

                Gradient gradient = new Gradient();
                gradient.SetKeys(
                    new GradientColorKey[] {
                        new GradientColorKey(trailColor, 0f),
                        new GradientColorKey(trailColor, 1f)
                    },
                    new GradientAlphaKey[] {
                        new GradientAlphaKey(0.5f, 0f),
                        new GradientAlphaKey(0f, 1f)
                    }
                );

                trailRenderer.colorGradient = gradient;
            }
        }
    }
}
