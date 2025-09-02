using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.ContentControl.Model
{
    public class AllUnlockable : IUnlockable, IDisposable
    {
        private readonly List<IUnlockable> _unlockables;

        public bool IsUnlocked => IsAllUnlocked();
        public string Condition => GetCondition();
        public event Action<IUnlockable> OnUnlocked;
        
        [Preserve]
        public AllUnlockable() => 
            _unlockables = new List<IUnlockable>();
        
        public AllUnlockable(IUnlockable unlockable)
        {
            _unlockables = new List<IUnlockable>();
            if(unlockable != null)
                _unlockables.Add(unlockable);
            SubscribeOnLocked();
        }

        public AllUnlockable(List<IUnlockable> unlockables)
        {
            _unlockables = unlockables;
            SubscribeOnLocked();
        }

        private void SubscribeOnLocked()
        {
            foreach (var unlockable in _unlockables)
            {
                if(!unlockable.IsUnlocked)
                    unlockable.OnUnlocked += OnUnlock;
            }
        }

        public void Add(IUnlockable unlockable)
        {
            if(unlockable == null) return;
            _unlockables.Add(unlockable);
            if(!unlockable.IsUnlocked)
                unlockable.OnUnlocked += OnUnlock;
        }

        private bool IsAllUnlocked()
        {
            foreach (var unlockable in _unlockables)
                if (!unlockable.IsUnlocked) return false;

            return true;
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
            if (IsAllUnlocked()) Unlock();
        }

        private void Unlock() => 
            OnUnlocked?.Invoke(this);

        public void Dispose()
        {
            foreach (var unlockable in _unlockables)
            {
                if(!unlockable.IsUnlocked)
                    unlockable.OnUnlocked -= OnUnlock;
            }
            _unlockables.Clear();
        }
    }
}