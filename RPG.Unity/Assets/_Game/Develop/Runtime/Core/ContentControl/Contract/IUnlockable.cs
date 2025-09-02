using System;

namespace PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract
{
    public interface IUnlockable
    {
        public bool IsUnlocked { get; }
        public string Condition { get; }
        public event Action<IUnlockable> OnUnlocked; 
    }
}