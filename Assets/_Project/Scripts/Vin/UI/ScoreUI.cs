using UnityEngine;
using TMPro;
using ColorOrCrash.Vin.Managers;

namespace ColorOrCrash.Vin.UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        private void Start()
        {
            GameManager.Instance.OnScoreChanged += UpdateScore;
            UpdateScore(0);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnScoreChanged -= UpdateScore;
            }
        }

        private void UpdateScore(int score)
        {
            scoreText.text = $"{score}";
        }
    }
}
