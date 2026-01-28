using UnityEngine;

namespace ColorOrCrash.Global.Models
{
    [CreateAssetMenu(fileName = "AudioData", menuName = "Scriptable Objects/AudioData")]
    public class AudioData : ScriptableObject
    {
        public string audioKey;
        public AudioClip clip;
        [Range(0, 1)] public float volume = 1f;
        [Range(0.5f, 1.5f)] public float pitch = 1f;
        public bool loop = false;
    }
}
