using System;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers
{
    public abstract class TutorialTrigger
    {
        public event Action OnTriggered;

        public abstract void Initialize();

        protected void Execute() => 
            OnTriggered?.Invoke();

        public abstract void Dispose();
    }
}