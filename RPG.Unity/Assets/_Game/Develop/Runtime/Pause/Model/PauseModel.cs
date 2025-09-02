using System;
using PleasantlyGames.RPG.Runtime.Pause.Type;

namespace PleasantlyGames.RPG.Runtime.Pause.Model
{
    internal sealed class PauseModel
    {
        private int _counter;

        private bool CanPause => _counter == 1;
        private bool CanContinue => _counter == 0;
        
        public bool IsPaused { get; private set; }
        public PauseType Type { get; private set; }
        
        public event Action<PauseModel> OnPause;
        public event Action<PauseModel> OnContinue;

        public PauseModel(PauseType pauseType) => 
            Type = pauseType;

        public void Pause()
        {
            _counter++;
            if (!CanPause) return;
            
            IsPaused = true;
            OnPause?.Invoke(this);
        }

        public void Continue()
        {
            _counter--;
            if (!CanContinue) return;
            
            IsPaused = false;
            OnContinue?.Invoke(this);
        }
    }
}