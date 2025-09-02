using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Type;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model
{
    public abstract class DungeonMode : SubMode, IGameMode
    {
        private bool _isLaunched;

        public abstract GameModeType Type { get; }
        public bool IsLaunched => _isLaunched; 
        
        public event Action<DungeonMode> OnExitRequested;
        public event Action<IGameMode> OnWin;
        public event Action<IGameMode> OnLose;
        public event Action<IGameMode> OnLaunched;
        public event Action<IGameMode> OnDisposed;
        
        [Preserve]
        public DungeonMode()
        {
            
        }

        public abstract void Leave();

        public virtual UniTask LaunchAsync()
        {
            _isLaunched = true;
            OnLaunched?.Invoke(this);
            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            base.Dispose();
            _isLaunched = false;
            OnDisposed?.Invoke(this);
        }
        
        public abstract string GetFormattedFullName();

        public string GetFormattedModeName() => 
            Translator.Translate(Type.ToString());

        protected virtual void Win()
        {
            SpendEnterResource();
            OnWin?.Invoke(this);
        }

        protected virtual void Lose() => 
            OnLose?.Invoke(this);

        protected void RequestExit() => 
            OnExitRequested?.Invoke(this);
    }
}