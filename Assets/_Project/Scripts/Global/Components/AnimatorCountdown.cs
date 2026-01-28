using UnityEngine;
using Cysharp.Threading.Tasks;
using NocturneThree.ServiceLocator;

namespace ColorOrCrash.Global.Components
{
    public class AnimatorCountdown : MonoBehaviour
    {
        [Header("GameObject References")]
        [SerializeField] private GameObject readyObject;
        [SerializeField] private GameObject goObject;
        
        [Header("Animator State Names")]
        [SerializeField] private string readyStateName = "Ready";
        [SerializeField] private string goStateName = "Go";
        
        [Header("Timing")]
        [SerializeField] private float readyDuration = 1.5f;
        [SerializeField] private float goDuration = 1f;
        
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip readySound;
        [SerializeField] private AudioClip goSound;

        private GameObject _loadingCanvas;

        private void Start()
        {
            // Pastikan game diam sejak frame pertama scene ini aktif
            Time.timeScale = 0;

            // Ambil referensi loading canvas dari LoadSceneManager
            var loadManager = ServiceLocator.Get<LoadSceneManager>();
            if (loadManager != null && loadManager.loadingCanvas != null)
            {
                _loadingCanvas = loadManager.loadingCanvas.gameObject;
            }

            if (readyObject != null) readyObject.SetActive(false);
            if (goObject != null) goObject.SetActive(false);
            
            // Jalankan urutan countdown
            StartSequence().Forget();
        }

        private async UniTaskVoid StartSequence()
        {
            // 1. Tunggu loading screen benar-benar OFF
            if (_loadingCanvas != null)
            {
                await UniTask.WaitUntil(() => !_loadingCanvas.activeInHierarchy);
                
                // BUFFER: Beri jeda 3 frame agar engine stabil setelah mematikan canvas berat
                await UniTask.DelayFrame(3);
                
                // Tambahkan sedikit nafas (0.2 detik) agar transisi visual tidak mengagetkan
                await UniTask.Delay(System.TimeSpan.FromSeconds(0.2f), ignoreTimeScale: true);
            }

            // 2. Jalankan READY
            if (readyObject != null)
            {
                await PlayAnimation(readyObject, readyStateName, readySound, readyDuration);
            }

            // 3. Jalankan GO
            if (goObject != null)
            {
                await PlayAnimation(goObject, goStateName, goSound, goDuration);
            }

            // 4. Mulai Game
            Time.timeScale = 1;
            
            // Beri jeda sedikit sebelum menghancurkan diri agar tidak ada lonjakan beban CPU mendadak
            await UniTask.DelayFrame(5);
            Destroy(gameObject);
        }

        private async UniTask PlayAnimation(GameObject obj, string stateName, AudioClip clip, float duration)
        {
            obj.SetActive(true);
            
            Animator anim = obj.GetComponent<Animator>();
            if (anim != null)
            {
                anim.updateMode = AnimatorUpdateMode.UnscaledTime;
                anim.Play(stateName, 0, 0f);
            }

            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }

            // Tunggu berdasarkan waktu nyata karena timeScale sedang 0
            await UniTask.Delay(System.TimeSpan.FromSeconds(duration), ignoreTimeScale: true);
            
            obj.SetActive(false);
        }
    }
}