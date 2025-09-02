using System;
using PleasantlyGames.RPG.Runtime.Audio.Definition;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Audio.Model
{
    [DisallowMultipleComponent, HideMonoScript, RequireComponent(typeof(AudioSource))]
    public class AudioEmitter : MonoBehaviour
    {
        [SerializeField, Required] private AudioSource _source;

        private bool _isPlaying;
        private bool _releaseOnEnd;
        private AudioService _service;
        
        private float _startPitch;
        private bool _pitchIsActive;
        private float _pitchEndTime;
        private int _pitchCounter;
        
        public AudioData Data { get; private set; }
        public event Action<AudioEmitter> OnStop;

        private void Reset() =>
            _source = GetComponent<AudioSource>();

        public void Initialize(AudioData data, AudioService service, bool releaseOnEnd)
        {
            Data = data;
            _service = service;
            _releaseOnEnd = releaseOnEnd;

            _startPitch = 0;
            _pitchCounter = 0;
            
            _source.outputAudioMixerGroup = data.MixerGroup;
            _source.loop = Data.Loop;
            _source.spatialBlend = Data.Effect3D;
        }

        private void ApplyData()
        {
            _source.clip = Data.RandomClip ? Data.RandomClips.GetRandomElement() : Data.Clip;
            if (Data.MaxPitchGrow <= 0 || _startPitch == 0) 
                _source.pitch = _startPitch =  Data.RandomPitch ? Data.PitchRange.Random() : Data.Pitch;
            else
                _source.pitch = IncreasePitch();
            _source.volume = Data.RandomVolume ? Data.VolumeRange.Random() : Data.Volume;
        }

        public void Play()
        {
            ApplyData();
            if(Data.PlayOneShot)
                _source.PlayOneShot(_source.clip);
            else
                _source.Play();
            _isPlaying = true;
        }

        public void Stop()
        {
            if (!_isPlaying) return;
            _source.Stop();
            _isPlaying = false;
            OnStop?.Invoke(this);
            if (_releaseOnEnd)
                _service.Release(this);
        }

        public void ReleaseOnEnd()
        {
            _releaseOnEnd = true;
            if (!_isPlaying) 
                _service.Release(this);
        }

        private float IncreasePitch()
        {
            if(_pitchCounter < Data.MaxPitchGrow)
                _pitchCounter++;
            ResetPitchTimer();
            return _startPitch + _pitchCounter * Data.PitchStep;
        }
        
        public void ResetPitchTimer()
        {
            _pitchEndTime = Time.time + Data.PitchLifetime;
            _pitchIsActive = true;
        }
        
        private void StopPitch()
        {
            _pitchIsActive = false;
            _pitchCounter = 0;
            _startPitch = 0;
        }

        private void Update()
        {
            if (_pitchIsActive)
            {
                if(Time.time > _pitchEndTime)
                    StopPitch();
            }
            
            if (!_isPlaying || _source.isPlaying) return;
            Stop();
        }
    }
}