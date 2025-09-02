using System;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Audio.Model;
using PleasantlyGames.RPG.Runtime.Audio.Type;

namespace PleasantlyGames.RPG.Runtime.Audio.Contract
{
    public interface IAudioService
    {
        UniTask LocalWarmUp();
        void Initialize();
        void TransitTo(AudioSnapshot snapshotType);
        AudioBuilder CreateLocalSound<T>(T audioEnum) where T : Enum;
        UniTask<AudioBuilder> CreateRemoteSound<T>(T audioEnum) where T : Enum;
        void SetVolume(AudioGroup group, float value);
    }
}