using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Bootstrap.LoadUnits
{
    public class AudioLoadUnit : ILoadUnit
    {
        private readonly IAudioService _audioService;

        public string DescriptionToken => "AudioLoading";

        [Preserve]
        public AudioLoadUnit(IAudioService audioService)
        {
            _audioService = audioService;
        }

        public async UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(0.7f);
            await _audioService.LocalWarmUp();
            progress?.Report(1);
        }
    }
}