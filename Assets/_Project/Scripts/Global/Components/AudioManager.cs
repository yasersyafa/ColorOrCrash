using System.Collections.Generic;
using System.Linq;
using ColorOrCrash.Global.Models;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NocturneThree.ServiceLocator;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

namespace ColorOrCrash.Global.Components
{
    [Service]
    public class AudioManager : MonoBehaviour, IGameService
    {
        [Header("Settings")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private List<AudioData> audioLibrary;

        [Header("BGM Source")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private float defaultFadeDuration = 1f;

        private IObjectPool<AudioSource> _sfxPool;
        private Dictionary<string, AudioData> _audioDict;

        private void Awake()
        {

            _audioDict = audioLibrary.ToDictionary(data => data.audioKey, data => data);

            _sfxPool = new ObjectPool<AudioSource>(
                createFunc: CreateAudioSource,
                actionOnGet: source => source.gameObject.SetActive(true),
                actionOnRelease: source => source.gameObject.SetActive(false),
                actionOnDestroy: source => Destroy(source.gameObject),
                defaultCapacity: 10,
                maxSize: 20
            );
        }

        private AudioSource CreateAudioSource()
        {
            GameObject go = new("SFX_Source");
            go.transform.SetParent(transform);
            AudioSource source = go.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
            return source;
        }

        #region Public API

        /// <summary>
        /// Play SFX by Key string.
        /// </summary>
        public void PlaySFX(string key)
        {
            if (!_audioDict.TryGetValue(key, out AudioData data))
            {
                Debug.LogWarning($"Could not found Audio key '{key}'!");
                return;
            }

            AudioSource source = _sfxPool.Get();
            source.clip = data.clip;
            source.volume = data.volume;
            source.pitch = data.pitch;
            source.loop = data.loop;
            source.Play();

            // release back to pool after finished playing
            ReturnToPoolAfterFinished(source, data.clip.length).Forget();
        }

        /// <summary>
        /// Play Background Music (Looping).
        /// </summary>
        public void PlayBGM(string key, bool fade = true)
        {
            if (!_audioDict.TryGetValue(key, out AudioData data)) return;

            bgmSource.loop = data.loop;
            bgmSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];

            if (fade)
            {
                float targetVol = data.volume;
                // Fade Out lagu lama, lalu Fade In lagu baru
                bgmSource.DOFade(0, defaultFadeDuration).SetUpdate(true).OnComplete(() =>
                {
                    bgmSource.clip = data.clip;
                    bgmSource.Play();
                    bgmSource.DOFade(targetVol, defaultFadeDuration).SetUpdate(true);
                });
            }
            else
            {
                bgmSource.clip = data.clip;
                bgmSource.volume = data.volume;
                bgmSource.Play();
            }
        }

        /// <summary>
        /// Stop Background Music
        /// </summary>
        /// <param name="fade">parameter for fade out the music</param>
        public void StopBGM(bool fade = true)
        {
            if (bgmSource == null || !bgmSource.isPlaying) return;

            if (fade)
            {
                bgmSource.DOFade(0, defaultFadeDuration)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        bgmSource.Stop();
                        bgmSource.clip = null;
                    });
            }
            else
            {
                bgmSource.Stop();
                bgmSource.clip = null;
            }
        }

        public void SetMasterVolume(float volume) => audioMixer.SetFloat("MasterVol", Mathf.Log10(volume) * 20);
        
        #endregion

        private async UniTaskVoid ReturnToPoolAfterFinished(AudioSource source, float duration)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(duration), ignoreTimeScale: true);
            if (source != null) _sfxPool.Release(source);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<AudioManager>();
        }
    }
}
