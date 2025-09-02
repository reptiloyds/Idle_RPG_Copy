using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Audio.Definition;
using PleasantlyGames.RPG.Runtime.Audio.Type;
using PleasantlyGames.RPG.Runtime.Pool;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using UnityEngine.Audio;
using VContainer;
using Object = UnityEngine.Object;

namespace PleasantlyGames.RPG.Runtime.Audio.Model
{
    public class AudioService : IAudioService
    {
        private readonly IAssetProvider _assetProvider;
        private readonly AudioConfig _config;

        private AudioMixer _mixer;
        private const string _noneAudioType = "None";
        private readonly Dictionary<string, AudioData> _audioDictionary = new();

        private ObjectPoolWithParent<AudioEmitter> _pool;
        private readonly List<AudioEmitter> _emitters = new();
        public readonly List<AudioEmitter> FrequentSoundEmitters = new();

        private readonly AudioBuilder _audioBuilder;

        [Preserve]
        [Inject]
        public AudioService(IAssetProvider assetProvider, AudioConfig config)
        {
            _assetProvider = assetProvider;
            _config = config;
            _audioBuilder = new AudioBuilder(this);
        }

        async UniTask IAudioService.LocalWarmUp()
        {
            var locations = await _assetProvider.GetResourceLocationsAsync(AssetLabel.LocalAudioData);
            var results = await _assetProvider.WarmUp<ScriptableObject>(locations);
            for (var i = 0; i < results.Length; i++)
            {
                var data = (AudioData)results[i];
                _audioDictionary[locations[i].PrimaryKey] = data;
            }

            _mixer = await _assetProvider.LoadAssetAsync<AudioMixer>(_config.AudioMixerRef, false);
        }

        void IAudioService.Initialize()
        {
            _pool = new ObjectPoolWithParent<AudioEmitter>("AudioSourcePool", Create, Get, ReleaseEmitter, Destroy,
                _config.DefaultCapacity, _config.MaxPoolSize);
            Object.DontDestroyOnLoad(_pool.Parent.gameObject);

            AudioEmitter Create()
            {
                var audioEmitter = Object.Instantiate(_config.AudioEmitterPrefab);
                audioEmitter.gameObject.SetActive(false);
                return audioEmitter;
            }

            void Get(AudioEmitter emitter)
            {
                emitter.gameObject.SetActive(true);
                _emitters.Add(emitter);
            }

            void ReleaseEmitter(AudioEmitter emitter)
            {
                emitter.gameObject.SetActive(false);
                _emitters.Remove(emitter);
            }

            void Destroy(AudioEmitter emitter)
            {
                _emitters.Remove(emitter);
                Object.Destroy(emitter);
            }
        }

        void IAudioService.SetVolume(AudioGroup group, float value) =>
            _mixer.SetFloat($"{group}Volume", Mathf.Lerp(-80, 0, value));

        public void TransitTo(AudioSnapshot snapshotType)
        {
            SnapshotSetup result = null;
            foreach (var setup in _config.SnapshotSetups)
            {
                if (setup.Type != snapshotType) continue;
                result = setup;
                break;
            }

            if (result == null) return;
            var snapshot = _mixer.FindSnapshot(result.Type.ToString());
            if (snapshot == null) return;
            snapshot.TransitionTo(result.TransitionTime);
        }

        public AudioBuilder CreateLocalSound<T>(T audioEnum) where T : Enum
        {
            var audioTypeName = audioEnum.ToString();
            var audio = CreateLocalSound(audioTypeName);
            return audio;
        }

        public async UniTask<AudioBuilder> CreateRemoteSound<T>(T audioEnum) where T : Enum
        {
            var audioTypeName = audioEnum.ToString();
            var audio = await CreateRemoteSound(audioTypeName);
            return audio;
        }

        public bool CanPlaySound(AudioData data)
        {
            if (data == null) return false;
            if (!data.FrequentSound) return true;
            if (FrequentSoundEmitters.Count >= _config.MaxSoundInstances)
            {
                var latest = FrequentSoundEmitters[0];
                Release(latest);
            }

            return true;
        }

        public AudioEmitter GetEmitter() =>
            _pool.Get();

        public void Release(AudioEmitter emitter)
        {
            if (emitter.Data.FrequentSound)
                FrequentSoundEmitters.Remove(emitter);
            _pool.Release(emitter);
        }

        private AudioBuilder CreateLocalSound(string audioTypeName)
        {
            if (string.Equals(audioTypeName, _noneAudioType))
                return _audioBuilder;
            if (_audioDictionary.TryGetValue(audioTypeName, out var data))
                return _audioBuilder.WithSoundData(data);

            return _audioBuilder;
        }

        private async UniTask<AudioBuilder> CreateRemoteSound(string audioTypeName)
        {
            if (string.Equals(audioTypeName, _noneAudioType))
                return _audioBuilder;
            if (!_audioDictionary.TryGetValue(audioTypeName, out var data))
            {
                var result = await _assetProvider.LoadAssetAsync<ScriptableObject>(audioTypeName, false);
                data = (AudioData)result;
                _audioDictionary[audioTypeName] = data;
            }

            return _audioBuilder.WithSoundData(data);
        }
    }
}