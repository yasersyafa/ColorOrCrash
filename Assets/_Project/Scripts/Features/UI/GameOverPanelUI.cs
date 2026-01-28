using ColorOrCrash.Global.Components;
using Cysharp.Threading.Tasks;
using NocturneThree.ServiceLocator;
using UnityEngine;

namespace ColorOrCrash.Features.UI
{
    public class GameOverPanelUI : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject buttonChoices, title;
        private GameManager manager;

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
            gameOverPanel.SetActive(true);
            await UniTask.Delay(500);
            
            ServiceLocator.Get<AudioManager>().PlaySFX("GameOver");
            title.SetActive(true);

            await UniTask.Delay(500);

            ServiceLocator.Get<AudioManager>().PlaySFX("GameOver");
            buttonChoices.SetActive(true);
        }

        public void OnRestartButtonPressed()
        {
            ServiceLocator.Get<AudioManager>().PlaySFX("Click");
            manager.ChangeState(GameState.Playing);
            Reset();
        }

        public async void OnExitButtonPressed()
        {
            ServiceLocator.Get<AudioManager>().PlaySFX("Click");
            await ServiceLocator.Get<LoadSceneManager>().LoadSceneAsync("MainMenuScene");
        }

        private void Reset()
        {
            gameOverPanel.SetActive(false);
            buttonChoices.SetActive(false);
            title.SetActive(false);
        }
    }
}
