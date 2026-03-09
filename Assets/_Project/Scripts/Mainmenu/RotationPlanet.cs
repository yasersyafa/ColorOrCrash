using UnityEngine;
using DG.Tweening;

namespace ColorOrCrash
{
    public class RotationPlanet : MonoBehaviour
    {
        public float duration = 5f;

        void Start()
        {
            transform.DORotate(new Vector3(0,0,360), duration, RotateMode.FastBeyond360)
                .SetUpdate(true)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear);
        }

        void OnDisable()
        {
            transform.DOKill();
        }
    }
}
