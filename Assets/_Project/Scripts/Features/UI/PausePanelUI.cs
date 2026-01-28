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
        // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            manager.ResumeGame();
            pausePanel.SetActive(false);
        }

        public async void OnExitButtonPressed()
        {
            await ServiceLocator.Get<LoadSceneManager>().LoadSceneAsync("MainMenuScene");
        }

        public void OnRestartButtonPressed()
        {
            manager.ChangeState(GameState.Playing);
            pausePanel.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            var keyboard = Keyboard.current;
            if(keyboard.escapeKey.wasPressedThisFrame && !manager.isPaused && manager.CurrentState == GameState.Playing)
            {
                manager.TogglePause();
            }
        }
    }
}
