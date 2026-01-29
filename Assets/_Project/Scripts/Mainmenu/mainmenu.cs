using ColorOrCrash.Global.Components;
using NocturneThree.ServiceLocator;
using UnityEngine;

namespace ColorOrCrash
{
    public class mainmenu : MonoBehaviour
    {
        [Header("Scene Settings")]
        [SerializeField] private string loadingSceneName = "LoadingScreen";
        
        [Header("UI References")]
        [SerializeField] private GameObject mainMenuContainer; // GameObject kosong yang berisi UI menu utama
        [SerializeField] private GameObject creditsPanel; // Panel untuk credits
        
        private void Start()
        {
            ServiceLocator.Get<AudioManager>().PlayBGM("MenuMusic");
            // Pastikan credits panel tidak aktif saat mulai
            if (creditsPanel != null)
            {
                creditsPanel.SetActive(false);
            }
            
            // Pastikan main menu container aktif saat mulai
            if (mainMenuContainer != null)
            {
                mainMenuContainer.SetActive(true);
            }
        }
        
        private void Update()
        {
            // Jika credits panel aktif dan tombol ESC ditekan, kembali ke main menu
            if (creditsPanel != null && creditsPanel.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    HideCredits();
                }
            }
        }
        
        /// <summary>
        /// Fungsi untuk button Start - Pindah ke scene loading screen
        /// </summary>
        public async void StartGame()
        {
            await ServiceLocator.Get<LoadSceneManager>().LoadSceneAsync(loadingSceneName);
        }
        
        /// <summary>
        /// Fungsi untuk button Credits - Toggle antara main menu dan credits
        /// </summary>
        public void ShowCredits()
        {
            if (mainMenuContainer != null)
            {
                mainMenuContainer.SetActive(false);
            }
            
            if (creditsPanel != null)
            {
                creditsPanel.SetActive(true);
            }
        }
        
        /// <summary>
        /// Fungsi untuk kembali dari credits ke main menu
        /// </summary>
        public void HideCredits()
        {
            if (creditsPanel != null)
            {
                creditsPanel.SetActive(false);
            }
            
            if (mainMenuContainer != null)
            {
                mainMenuContainer.SetActive(true);
            }
        }
        
        /// <summary>
        /// Fungsi untuk button Exit - Keluar dari game
        /// </summary>
        public void ExitGame()
        {
            Debug.Log("Exiting game...");
            
            #if UNITY_EDITOR
                // Jika di Unity Editor, stop play mode
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                // Jika di build, quit application
                Application.Quit();
            #endif
        }
    }
}
