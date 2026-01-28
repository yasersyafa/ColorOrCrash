using UnityEngine;

namespace ColorOrCrash.Vin.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource musicSource;

        [Header("Sound Effects")]
        [SerializeField] private AudioClip collectSound;
        [SerializeField] private AudioClip colorChangeSound;
        [SerializeField] private AudioClip dashSound;
        [SerializeField] private AudioClip gameOverSound;

        [Header("Music")]
        [SerializeField] private AudioClip backgroundMusic;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            PlayBackgroundMusic();
        }

        public void PlayCollectSound()
        {
            PlaySFX(collectSound);
        }

        public void PlayColorChangeSound()
        {
            PlaySFX(colorChangeSound);
        }

        public void PlayDashSound()
        {
            PlaySFX(dashSound);
        }

        public void PlayGameOverSound()
        {
            PlaySFX(gameOverSound);
        }

        private void PlaySFX(AudioClip clip)
        {
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }

        public void PlayBackgroundMusic()
        {
            if (backgroundMusic != null && musicSource != null)
            {
                musicSource.clip = backgroundMusic;
                musicSource.loop = true;
                musicSource.Play();
            }
        }

        public void StopBackgroundMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }

        public void SetMusicVolume(float volume)
        {
            if (musicSource != null)
            {
                musicSource.volume = Mathf.Clamp01(volume);
            }
        }

        public void SetSFXVolume(float volume)
        {
            if (sfxSource != null)
            {
                sfxSource.volume = Mathf.Clamp01(volume);
            }
        }
    }
}
