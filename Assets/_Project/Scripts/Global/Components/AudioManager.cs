using System.Collections.Generic;
using System.Linq;
using ColorOrCrash.Global.Models;
using Cysharp.Threading.Tasks;
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
        /// Memutar SFX berdasarkan Key string.
        /// </summary>
        public void PlaySFX(string key)
        {
            if (!_audioDict.TryGetValue(key, out AudioData data))
            {
                Debug.LogWarning($"Audio key '{key}' tidak ditemukan!");
                return;
            }

            AudioSource source = _sfxPool.Get();
            source.clip = data.clip;
            source.volume = data.volume;
            source.pitch = data.pitch;
            source.loop = data.loop;
            source.Play();

            // Kembalikan ke pool setelah selesai diputar
            ReturnToPoolAfterFinished(source, data.clip.length).Forget();
        }

        /// <summary>
        /// Memutar Background Music (Looping).
        /// </summary>
        public void PlayBGM(string key, bool fade = true)
        {
            if (!_audioDict.TryGetValue(key, out AudioData data)) return;

            bgmSource.clip = data.clip;
            bgmSource.volume = data.volume;
            bgmSource.loop = true;
            bgmSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
            bgmSource.Play();
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
