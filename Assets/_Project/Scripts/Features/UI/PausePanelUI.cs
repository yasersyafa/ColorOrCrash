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

        public void OnExitButtonPressed()
        {
            
        }

        public void OnRestartButtonPressed()
        {

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
