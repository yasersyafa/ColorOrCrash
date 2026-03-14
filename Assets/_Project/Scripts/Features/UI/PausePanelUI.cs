using ColorOrCrash.Global.Components;
using NocturneThree.ServiceLocator;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ColorOrCrash
{
    public class PausePanelUI : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;
        private GameManager manager;

        void Start()
        {
            manager = ServiceLocator.Get<GameManager>();
            manager.OnGamePaused += () =>
            {
                pausePanel.SetActive(true);
            };
        }

        public void OnResumeButtonPressed()
        {
            ServiceLocator.Get<AudioManager>().PlaySFX("Click");
            pausePanel.SetActive(false);
            PokiService.Instance.CommercialBreak(() =>
            {
                manager.ResumeGame();
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

        public void OnRestartButtonPressed()
        {
            ServiceLocator.Get<AudioManager>().PlaySFX("Click");
            pausePanel.SetActive(false);
            PokiService.Instance.CommercialBreak(() =>
            {
                manager.ChangeState(GameState.Countdown);
            });
        }

        void Update()
        {
            var keyboard = Keyboard.current;
            if(keyboard != null && keyboard.escapeKey.wasPressedThisFrame && !manager.isPaused && manager.CurrentState == GameState.Playing)
            {
                manager.TogglePause();
            }
        }
    }
}
