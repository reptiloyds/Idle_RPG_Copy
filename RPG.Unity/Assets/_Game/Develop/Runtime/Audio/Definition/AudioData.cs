using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace PleasantlyGames.RPG.Runtime.Audio.Definition
{
    [CreateAssetMenu(fileName = "AudioData", menuName = "SO/AudioData")]
    public class AudioData : ScriptableObject
    {
        public bool RandomClip;
        [HideIf("@this.RandomClip == true")]
        public AudioClip Clip;
        [HideIf("@this.RandomClip == false")]
        public AudioClip[] RandomClips;
        public AudioMixerGroup MixerGroup;
        public bool Loop;
        public bool FrequentSound;
        public bool PlayOneShot;
        public bool RandomVolume;
        [HideIf("@this.RandomVolume == true")] [UnityEngine.Range(0, 1)]
        public float Volume = 0.5f;
        [HideIf("@this.RandomVolume == false")]
        public Vector2 VolumeRange = new(0.4f, 0.5f);
        public bool RandomPitch;
        [HideIf("@this.RandomPitch == true")] [UnityEngine.Range(0, 2)]
        public float Pitch = 1f;
        [HideIf("@this.RandomPitch == false")]
        public Vector2 PitchRange = new(0.9f, 1.1f);
        [MinValue(0)]
        public int MaxPitchGrow;
        [MinValue(0)] [HideIf("@this.MaxPitchGrow <= 0")]
        public float PitchStep;
        [MinValue(0)] [HideIf("@this.MaxPitchGrow <= 0")]
        public float PitchLifetime;
        [UnityEngine.Range(0, 1)]
        public float Effect3D = 0;
    }
}