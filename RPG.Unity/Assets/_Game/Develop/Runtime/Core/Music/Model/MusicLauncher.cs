using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Audio.Model;
using PleasantlyGames.RPG.Runtime.Core.Music.Definition;
using PleasantlyGames.RPG.Runtime.Core.Music.Type;
using PleasantlyGames.RPG.Runtime.UnityExtension;
using UnityEngine.Scripting;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Music.Model
{
    public class MusicLauncher
    {
        [Inject] private MusicConfiguration _configuration;
        [Inject] private IAudioService _audioService;

        private AudioEmitter _audioEmitter;
        private int _sequenceId;
        
        [UnityEngine.Scripting.Preserve]
        public MusicLauncher()
        {
            
        }

        public void Initialize()
        {
            switch (_configuration.Mode)
            {
                case MusicMode.None:
                    return;
                case MusicMode.Single:
                    LaunchSingle();
                    break;
                case MusicMode.Sequence:
                    LaunchSequence();
                    break;
                case MusicMode.Random:
                    LaunchRandom();
                    break;
            }
        }

        private async void LaunchSingle()
        {
            _audioEmitter = (await _audioService.CreateRemoteSound(_configuration.MusicType)).Build();
            _audioEmitter.OnStop += OnSingleMusicStop;
            _audioEmitter.Play();
        }

        private void OnSingleMusicStop(AudioEmitter audioEmitter)
        {
            audioEmitter.OnStop -= OnSingleMusicStop;
            LaunchSingle();
        }

        private async void LaunchSequence()
        {
            if(_configuration.MusicList.Count == 0) return;
            
            _audioEmitter = (await _audioService.CreateRemoteSound(_configuration.MusicList[_sequenceId])).Build();
            _audioEmitter.OnStop += OnSequenceStop;
            _audioEmitter.Play();
        }

        private void OnSequenceStop(AudioEmitter audioEmitter)
        {
            audioEmitter.OnStop -= OnSequenceStop;
            _sequenceId++;
            if (_configuration.MusicList.Count >= _sequenceId) 
                _sequenceId = 0;
            LaunchSequence();
        }
        
        private async void LaunchRandom()
        {
            if(_configuration.MusicList.Count == 0) return;
            
            _audioEmitter = (await _audioService.CreateRemoteSound(_configuration.MusicList.GetRandomElement())).Build();
            _audioEmitter.OnStop += OnRandomStop;
            _audioEmitter.Play();
        }

        private void OnRandomStop(AudioEmitter audioEmitter)
        {
            audioEmitter.OnStop -= OnRandomStop;
            LaunchRandom();
        }
    }
}