using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Audio.Model;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace PleasantlyGames.RPG.Runtime.Audio.Definition
{
    [Serializable]
    public class AudioConfig
    {
        public AssetReferenceT<AudioMixer> AudioMixerRef;
        public int DefaultCapacity = 10;
        public int MaxPoolSize = 100;
        public int MaxSoundInstances = 30;
        public List<SnapshotSetup> SnapshotSetups;
        public AudioEmitter AudioEmitterPrefab;
    }
}