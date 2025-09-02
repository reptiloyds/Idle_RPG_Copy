using System;
using PleasantlyGames.RPG.Runtime.Pause.Type;

namespace PleasantlyGames.RPG.Runtime.Pause.Contract
{
    public interface IPauseService
    {
        event Action<PauseType> OnPause;
        event Action<PauseType> OnContinue;
        
        bool IsPauseEnabled(PauseType pauseType);
        void Pause(PauseType pauseType);
        void Continue(PauseType pauseType);
    }
}