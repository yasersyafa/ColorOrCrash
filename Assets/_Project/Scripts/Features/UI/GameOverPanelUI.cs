using ColorOfCrash.Utils;
using ColorOrCrash.Global.Components;
using Cysharp.Threading.Tasks;
using NocturneThree.ServiceLocator;
using TMPro;
using UnityEngine;

namespace ColorOrCrash.Features.UI
{
    public class GameOverPanelUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject buttonChoices, title, panelStats;
        [SerializeField] private TextMeshProUGUI redText, greenText, blueText, totalText;

        [Header("Rewarded Ad Buttons")]
        [SerializeField] private GameObject doubleCoinsButton;
        [SerializeField] private TextMeshProUGUI doubleCoinsLabel;

        private GameManager manager;
        private bool _hasDoubledCoins;

        void Start()
        {
            Reset();
            manager = ServiceLocator.Get<GameManager>();
            manager.OnGameStateChanged += HandleGameState;
        }

        void OnDestroy()
        {
            manager.OnGameStateChanged -= HandleGameState;
        }

        private async void HandleGameState(GameState state)
        {
            if(state != GameState.GameOver)
            {
                Reset();
                return;
            }

            await ShowGameOverPanel();
        }

        private async UniTask ShowGameOverPanel()
        {
            var ct = this.GetCancellationTokenOnDestroy();

            gameOverPanel.SetActive(true);
            await UniTask.Delay(500, cancellationToken: ct);

            ServiceLocator.Get<AudioManager>().PlaySFX("GameOver");
            title.SetActive(true);

            await UniTask.Delay(500, cancellationToken: ct);

            ServiceLocator.Get<AudioManager>().PlaySFX("GameOver");
            panelStats.SetActive(true);

            int red = manager.RedPoint;
            int green = manager.GreenPoint;
            int blue = manager.BluePoint;
            int totalScore = red + green + blue;

            float sfxDuration = ServiceLocator.Get<AudioManager>().GetAudioLength("Countup");
            if (sfxDuration <= 0) sfxDuration = 0.8f;

            ServiceLocator.Get<AudioManager>().FadeBGMVolume(0.3f, 0.5f);

            ServiceLocator.Get<AudioManager>().PlaySFX("CountUp");
            await redText.CountUpAsync(0, red, sfxDuration, ct);

            ServiceLocator.Get<AudioManager>().PlaySFX("CountUp");
            await greenText.CountUpAsync(0, green, sfxDuration, ct);

            ServiceLocator.Get<AudioManager>().PlaySFX("CountUp");
            await blueText.CountUpAsync(0, blue, sfxDuration, ct);

            totalText.SetText("Total: " + totalScore.ToString());
            ServiceLocator.Get<AudioManager>().PlaySFX("GameOver");
            totalText.gameObject.SetActive(true);

            await UniTask.Delay(500, cancellationToken: ct);

            ServiceLocator.Get<AudioManager>().PlaySFX("GameOver");
            buttonChoices.SetActive(true);

            // Show double coins button only if ads are not blocked and not already used
            if (doubleCoinsButton != null)
            {
                bool showReward = !_hasDoubledCoins && !PokiService.Instance.IsAdBlocked();
                doubleCoinsButton.SetActive(showReward);
            }
        }

        public void OnRestartButtonPressed()
        {
            ServiceLocator.Get<AudioManager>().PlaySFX("Click");
            PokiService.Instance.CommercialBreak(() =>
            {
                manager.ChangeState(GameState.Countdown);
                Reset();
            });
        }

        public void OnExitButtonPressed()
        {
            ServiceLocator.Get<AudioManager>().PlaySFX("Click");
            PokiService.Instance.CommercialBreak(async () =>
            {
                ServiceLocator.Get<AudioManager>().StopBGM();
                await ServiceLocator.Get<LoadSceneManager>().LoadSceneAsync(ServiceContainer.Instance.Scenes.MainMenuScene);
            });
        }

        public void OnDoubleCoinsButtonPressed()
        {
            if (_hasDoubledCoins) return;

            ServiceLocator.Get<AudioManager>().PlaySFX("Click");
            PokiService.Instance.RewardedBreak((withReward) =>
            {
                if (!withReward) return;

                _hasDoubledCoins = true;
                if (doubleCoinsButton != null)
                    doubleCoinsButton.SetActive(false);

                var save = ServiceLocator.Get<SaveManager>();
                int red = manager.RedPoint;
                int green = manager.GreenPoint;
                int blue = manager.BluePoint;

                save.Data.redCoins += red;
                save.Data.greenCoins += green;
                save.Data.blueCoins += blue;
                save.SaveGame();

                int doubled = (red + green + blue) * 2;
                totalText.SetText("Total: " + doubled.ToString());
                ServiceLocator.Get<AudioManager>().PlaySFX("Score");
            });
        }

        private void Reset()
        {
            _hasDoubledCoins = false;
            redText.text = "0";
            greenText.text = "0";
            blueText.text = "0";

            gameOverPanel.SetActive(false);
            buttonChoices.SetActive(false);
            title.SetActive(false);
            totalText.gameObject.SetActive(true);
            panelStats.SetActive(false);

            if (doubleCoinsButton != null)
                doubleCoinsButton.SetActive(false);
        }
    }
}
