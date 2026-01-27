using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ColorOrCrash.Features.TextFloat.Components
{
    public class TextFloat : MonoBehaviour
    {
        [SerializeField] private TMP_Text targetText;

        [SerializeField] private float duration = 0.8f;
        [SerializeField] private float jumpPower = .5f;
        [SerializeField] private float driftDistance = 0.15f;

        private CancellationTokenSource _cts;
        private Action<TextFloat> _onComplete;
        
        public void Setup(string message, Action<TextFloat> returnAction)
        {
            _cts?.Cancel();
            _cts = new();

            targetText.SetText(message);
            _onComplete = returnAction;

            transform.localScale = Vector3.zero;
            targetText.alpha = 1f;

            AnimateAsync(_cts.Token).Forget();
        }

        private async UniTask AnimateAsync(CancellationToken token)
        {
            try
            {
                var scaleTween = transform.DOScale(1f, duration * .4f)
                    .SetEase(Ease.OutBack)
                    .SetUpdate(true)
                    .ToUniTask(cancellationToken: token);
                
                Vector3 targetPos = transform.position + new Vector3(UnityEngine.Random.Range(-driftDistance, driftDistance), 1f, 0);
                var jumpTween = transform.DOJump(targetPos, jumpPower, 1, duration)
                    .SetEase(Ease.OutQuad)
                    .SetUpdate(true)
                    .ToUniTask(cancellationToken: token);
                
                await UniTask.WhenAll(scaleTween, jumpTween);

                await targetText.DOFade(0f, duration * .4f)
                    .SetEase(Ease.InSine)
                    .SetUpdate(true)
                    .ToUniTask(cancellationToken: token);
                
                _onComplete?.Invoke(this);
            }
            catch (OperationCanceledException)
            {
                // Task was cancelled, ignore
            }
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            
            transform.DOKill();
        }
    }
}
