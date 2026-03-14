using ColorOrCrash.Global.Components;
using Cysharp.Threading.Tasks;
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
            var poki = ServiceLocator.Get<PokiService>();
            if (poki != null)
            {
                poki.CommercialBreak(() => manager.ResumeGame());
            }
            else
            {
                manager.ResumeGame();
            }
        }

        public void OnExitButtonPressed()
        {
            ServiceLocator.Get<AudioManager>().PlaySFX("Click");
            var poki = ServiceLocator.Get<PokiService>();
            if (poki != null)
            {
                poki.CommercialBreak(async () =>
                {
                    ServiceLocator.Get<AudioManager>().StopBGM();
                    await ServiceLocator.Get<LoadSceneManager>().LoadSceneAsync(ServiceContainer.Instance.Scenes.MainMenuScene);
                });
            }
            else
            {
                ServiceLocator.Get<AudioManager>().StopBGM();
                ServiceLocator.Get<LoadSceneManager>().LoadSceneAsync(ServiceContainer.Instance.Scenes.MainMenuScene).Forget();
            }
        }

        public void OnRestartButtonPressed()
        {
            ServiceLocator.Get<AudioManager>().PlaySFX("Click");
            pausePanel.SetActive(false);
            var poki = ServiceLocator.Get<PokiService>();
            if (poki != null)
            {
                poki.CommercialBreak(() => manager.ChangeState(GameState.Countdown));
            }
            else
            {
                manager.ChangeState(GameState.Countdown);
            }
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
