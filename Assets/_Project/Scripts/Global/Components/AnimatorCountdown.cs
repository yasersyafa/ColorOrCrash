using UnityEngine;
using System.Collections;

namespace ColorOrCrash.Global.Components
{
    /// <summary>
    /// Countdown untuk 2 GameObject dengan Animator
    /// Cukup assign GameObject dan nama state di Animator
    /// </summary>
    public class AnimatorCountdown : MonoBehaviour
    {
        [Header("GameObject References")]
        [SerializeField] private GameObject readyObject;
        [SerializeField] private GameObject goObject;
        
        [Header("Animator State Names")]
        [Tooltip("Nama state di Animator untuk Ready (kosongkan jika default state)")]
        [SerializeField] private string readyStateName = "";
        
        [Tooltip("Nama state di Animator untuk Go (kosongkan jika default state)")]
        [SerializeField] private string goStateName = "";
        
        [Header("Timing")]
        [SerializeField] private float readyDuration = 1.5f;
        [SerializeField] private float goDuration = 1f;
        
        [Header("Loading Transition (Optional)")]
        [Tooltip("Canvas loading transition (opsional, untuk tunggu fade out selesai)")]
        [SerializeField] private GameObject loadingCanvas;
        
        [Header("Audio (Optional)")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip readySound;
        [SerializeField] private AudioClip goSound;
        
        private void Start()
        {
            if (readyObject != null) readyObject.SetActive(false);
            if (goObject != null) goObject.SetActive(false);
            
            StartCoroutine(WaitAndStart());
        }
        
        private IEnumerator WaitAndStart()
        {
            // Tunggu loading canvas hilang (jika ada)
            if (loadingCanvas != null)
            {
                Debug.Log("[AnimatorCountdown] Waiting for loading transition...");
                yield return new WaitUntil(() => !loadingCanvas.activeInHierarchy);
                Debug.Log("[AnimatorCountdown] Loading transition done");
            }
            
            // Baru pause game
            Time.timeScale = 0;
            Debug.Log("[AnimatorCountdown] Game paused");
            
            // Mulai countdown
            StartCoroutine(CountdownSequence());
        }
        
        private IEnumerator CountdownSequence()
        {
            // === READY ===
            if (readyObject != null)
            {
                Debug.Log("[AnimatorCountdown] Showing Ready...");
                readyObject.SetActive(true);
                
                // Setup Animator
                Animator animator = readyObject.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.updateMode = AnimatorUpdateMode.UnscaledTime;
                    
                    // Play state jika ada nama state
                    if (!string.IsNullOrEmpty(readyStateName))
                    {
                        animator.Play(readyStateName, 0, 0f);
                        Debug.Log($"[AnimatorCountdown] Playing state: {readyStateName}");
                    }
                    else
                    {
                        Debug.Log("[AnimatorCountdown] Using default state");
                    }
                }
                
                // Play sound
                if (audioSource != null && readySound != null)
                {
                    audioSource.PlayOneShot(readySound);
                }
                
                // Wait
                yield return new WaitForSecondsRealtime(readyDuration);
                
                readyObject.SetActive(false);
                Debug.Log("[AnimatorCountdown] Ready selesai");
            }
            
            // === GO ===
            if (goObject != null)
            {
                Debug.Log("[AnimatorCountdown] Showing Go...");
                goObject.SetActive(true);
                
                // Setup Animator
                Animator animator = goObject.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.updateMode = AnimatorUpdateMode.UnscaledTime;
                    
                    // Play state jika ada nama state
                    if (!string.IsNullOrEmpty(goStateName))
                    {
                        animator.Play(goStateName, 0, 0f);
                        Debug.Log($"[AnimatorCountdown] Playing state: {goStateName}");
                    }
                    else
                    {
                        Debug.Log("[AnimatorCountdown] Using default state");
                    }
                }
                
                // Play sound
                if (audioSource != null && goSound != null)
                {
                    audioSource.PlayOneShot(goSound);
                }
                
                // Wait
                yield return new WaitForSecondsRealtime(goDuration);
                
                goObject.SetActive(false);
                Debug.Log("[AnimatorCountdown] Go selesai");
            }
            
            // Resume
            Time.timeScale = 1;
            Debug.Log("[AnimatorCountdown] Game dimulai!");
            
            Destroy(gameObject, 0.5f);
        }
    }
}
