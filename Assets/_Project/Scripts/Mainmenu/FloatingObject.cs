using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace ColorOrCrash.MainMenu
{
    public class FloatingObject : MonoBehaviour
    {
        [Header("Floating Settings")]
        [SerializeField] private float amplitude = 0.5f;
        [SerializeField] private float duration = 2f;
        [SerializeField] private Ease floatEase = Ease.InOutSine;

        [Header("Rotation Settings")]
        [SerializeField] private Vector3 startRotation = new(0, 0, 360);
        [SerializeField] private float rotationDuration = 10f;

        private RectTransform rect;

        private void Start()
        {
            rect = GetComponent<RectTransform>();
            StartSequence().Forget();
        }

        private async UniTaskVoid StartSequence()
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(Random.Range(0f, 1f)), ignoreTimeScale: true);
            
            StartFloatingAnimation();
            StartRotationAnimation();
        }

        private void StartFloatingAnimation()
        {
            rect.DOLocalMoveY(rect.anchoredPosition.y + amplitude, duration)
                .SetEase(floatEase)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }

        private void StartRotationAnimation()
        {
            float swayAngle = 15f;

            transform.localRotation = Quaternion.Euler(startRotation);

            transform.DORotate(new Vector3(0, 0, swayAngle), rotationDuration)
                .SetEase(Ease.InOutQuad)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }
    }
}
