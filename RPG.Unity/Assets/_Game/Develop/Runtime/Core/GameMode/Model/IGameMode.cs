using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Type;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model 
{
    public interface IGameMode
    {
        GameModeType Type { get; }
        bool IsLaunched { get; } 
        
        event Action<IGameMode> OnWin;
        event Action<IGameMode> OnLose;
        event Action<IGameMode> OnLaunched;
        event Action<IGameMode> OnDisposed;

        void Initialize();
        UniTask LaunchAsync();
        void Dispose();
        void Restart() {}
        void OnResultPresented(){}
        
        virtual void OnResultWasClosed(){}
        
        string GetFormattedFullName();
        string GetFormattedModeName();
    }
}