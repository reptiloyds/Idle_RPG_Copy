using PleasantlyGames.RPG.Runtime.Audio.Definition;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Audio.Model
{
    public class AudioBuilder
    {
        private readonly AudioService _audioService;

        private AudioData _data;
        private Vector3 _position = Vector3.zero;
        private bool _releaseOnEnd = true;

        public AudioBuilder(AudioService audioService) =>
            _audioService = audioService;

        public AudioBuilder WithSoundData(AudioData data)
        {
            _data = data;
            return this;
        }

        public AudioBuilder WithPosition(Vector3 position)
        {
            _position = position;
            return this;
        }

        public AudioBuilder DontRelease()
        {
            _releaseOnEnd = false;
            return this;
        }

        public AudioEmitter Build()
        {
            var emitter = _audioService.GetEmitter();
            emitter.Initialize(_data, _audioService, _releaseOnEnd);
            emitter.transform.position = _position;
            if (_data.FrequentSound) 
                _audioService.FrequentSoundEmitters.Add(emitter);
            
            CleanUp();

            return emitter;
        }

        public void Play()
        {
            if (_audioService.CanPlaySound(_data))
            {
                var emitter = _audioService.GetEmitter();
                emitter.Initialize(_data, _audioService, _releaseOnEnd);
                emitter.transform.position = _position;
                if (_data.FrequentSound) 
                    _audioService.FrequentSoundEmitters.Add(emitter);
                
                emitter.Play();   
            }

            CleanUp();
        }

        private void CleanUp()
        {
            _data = null;
            _position = Vector3.zero;
            _releaseOnEnd = true;
        }
    }
}