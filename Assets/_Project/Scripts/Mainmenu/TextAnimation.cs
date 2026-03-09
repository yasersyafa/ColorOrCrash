using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ColorOrCrash
{
    public class TextAnimation : MonoBehaviour
    {
        private TextMeshProUGUI textObject;
        
        void OnEnable()
        {
            if(!gameObject.activeInHierarchy)
                gameObject.SetActive(true);
            textObject = GetComponent<TextMeshProUGUI>();

            textObject.DOFade(0, 1f)
                .SetUpdate(true)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        void OnDisable()
        {
            textObject.DOKill();
            gameObject.SetActive(false);
        }
    }
}
