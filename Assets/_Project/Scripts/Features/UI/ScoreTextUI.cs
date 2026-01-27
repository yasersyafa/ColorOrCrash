using ColorOrCrash.Global.Components;
using DG.Tweening;
using NocturneThree.ServiceLocator;
using TMPro;
using UnityEngine;

namespace ColorOrCrash.Features.UI
{
    public class ScoreTextUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI scoreText;
        
        [Header("Settings")]
        [SerializeField] private float countDuration = 0.5f;
        [SerializeField] private float punchScaleAmount = 0.2f;

        private GameManager _manager;

        private int _displayedScore = 0;
        private Tween _scoreTween;

        private void Start()
        {
            _manager = ServiceLocator.Get<GameManager>();

            scoreText.text = "0";
            
            _manager.OnScoreAdded += HandleScoreAdded;
        }

        private void OnDestroy()
        {
            _manager.OnScoreAdded -= HandleScoreAdded;
            _scoreTween?.Kill();
        }

        private void HandleScoreAdded(int addedAmount, int currentTotalScore)
        {
            _scoreTween?.Kill();
            
            _scoreTween = DOTween.To(() => _displayedScore, x => _displayedScore = x, currentTotalScore, countDuration)
                .OnUpdate(() => 
                {
                    scoreText.text = _displayedScore.ToString();
                })
                .SetEase(Ease.OutQuad);

            scoreText.transform.DOPunchScale(Vector3.one * punchScaleAmount, 0.2f, 10, 1)
                .OnComplete(() => scoreText.transform.localScale = Vector3.one);
        }
    }
}
