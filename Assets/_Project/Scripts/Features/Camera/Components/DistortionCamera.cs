using Cysharp.Threading.Tasks;
using DG.Tweening;
using NocturneThree.ServiceLocator;
using UnityEngine;

namespace ColorOrCrash.Features.Camera.Components
{
    public class DistortionCamera : MonoBehaviour, IGameService
    {
        [SerializeField] private Material materialEffect;
        [SerializeField] private float duration = 0.5f;

        private static string shockWavePropertyName = "_ShockWaveStrength";

        private void Awake()
        {
            ServiceLocator.Register<DistortionCamera>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unregister<DistortionCamera>();
        }

#region Public API Methods

        public async UniTask ApplyEffect()
        {
            materialEffect.SetFloat(shockWavePropertyName, 0);

            await DOTween.Sequence()
                .Append(materialEffect.DOFloat(-0.1f, shockWavePropertyName, duration * 0.3f))
                .Append(materialEffect.DOFloat(0, shockWavePropertyName, duration * 0.7f))
                .SetEase(Ease.OutQuad)
                .ToUniTask();
        }

#endregion
    }
}
