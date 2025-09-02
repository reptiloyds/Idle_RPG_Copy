using System;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.CompleteConditions
{
    public abstract class TutorialCondition
    {
        public event Action OnComplete;

        public virtual void Initialize()
        {
        }

        public virtual void Dispose()
        {
        }
        
        protected void Complete() => 
            OnComplete?.Invoke();

        public abstract void Pause();
        public abstract void Continue();
    }
}