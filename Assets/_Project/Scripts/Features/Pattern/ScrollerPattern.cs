using UnityEngine;
using UnityEngine.UI;

namespace ColorOrCrash.Features.Pattern
{
    public class ScrollerPattern : MonoBehaviour
    {
        [SerializeField] private RawImage _rawImage;
        [SerializeField] private float _x, _y;

        // Update is called once per frame
        void Update()
        {
            _rawImage.uvRect = new(_rawImage.uvRect.position + new Vector2(_x, _y) * Time.unscaledDeltaTime, _rawImage.uvRect.size);
        }
    }
}
