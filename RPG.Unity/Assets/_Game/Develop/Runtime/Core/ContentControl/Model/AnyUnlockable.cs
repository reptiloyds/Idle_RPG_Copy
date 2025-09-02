using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;

namespace PleasantlyGames.RPG.Runtime.Core.ContentControl.Model
{
    public class AnyUnlockable : IUnlockable, IDisposable
    {
        private readonly List<IUnlockable> _unlockables = new List<IUnlockable>();

        public bool IsUnlocked => IsAnyUnlocked();
        public string Condition => GetCondition();
        public event Action<IUnlockable> OnUnlocked;

        public AnyUnlockable()
        {
        }
        
        public AnyUnlockable(IUnlockable unlockable) => 
            Add(unlockable);

        public AnyUnlockable(IReadOnlyList<IUnlockable> unlockables)
        {
            foreach (var unlockable in unlockables) 
                Add(unlockable);
        }

        public void Add(IUnlockable unlockable)
        {
            if(unlockable == null) return;
            _unlockables.Add(unlockable);
            if (!IsUnlocked) 
                unlockable.OnUnlocked += OnUnlock;
        }

        private bool IsAnyUnlocked()
        {
            foreach (var unlockable in _unlockables)
                if (unlockable.IsUnlocked) return true;

            return false;
        }

        private string GetCondition()
        {
            foreach (var unlockable in _unlockables)
                if (!unlockable.IsUnlocked) return unlockable.Condition;

            return string.Empty;
        }

        private void OnUnlock(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnUnlock;
            Unlock();
        }

        private void Unlock()
        {
            foreach (var unlockable in _unlockables)
                if(!unlockable.IsUnlocked)
                    unlockable.OnUnlocked -= OnUnlock;
            OnUnlocked?.Invoke(this);
        }

        public void Dispose()
        {
            foreach (var unlockable in _unlockables)
                if(!unlockable.IsUnlocked)
                    unlockable.OnUnlocked -= OnUnlock;
            _unlockables.Clear();
        }
    }
}