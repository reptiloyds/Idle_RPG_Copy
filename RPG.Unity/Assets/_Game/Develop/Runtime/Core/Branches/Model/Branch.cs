using System;
using PleasantlyGames.RPG.Runtime.Core.Branches.Save;
using PleasantlyGames.RPG.Runtime.Core.Branches.Sheet;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Branches.Model
{
    public sealed class Branch : IUnlockable, IDisposable
    {
        private readonly BranchData _data;
        private readonly BranchSheet.Row _config;
        private readonly IUnlockable _unlockable;

        public string Id => _data.Id;
        public string DefaultCharacterId => _config.CharacterId;
        public string CharacterId => _data.CharacterId;
        
        public bool IsUnlocked => _unlockable == null || _unlockable.IsUnlocked;
        public string Condition => _unlockable == null ? string.Empty : _unlockable.Condition;
        public Sprite Sprite { get; private set; }
        public event Action<IUnlockable> OnUnlocked;
        
        public Branch(BranchData data, BranchSheet.Row config, Sprite sprite, IUnlockable unlockable)
        {
            _data = data;
            _config = config;
            Sprite = sprite;
            
            _unlockable = unlockable;
            if (!IsUnlocked) 
                _unlockable.OnUnlocked += OnUnlock;
        }
        
        public void Dispose()
        {
            if(!IsUnlocked)
                _unlockable.OnUnlocked -= OnUnlock;
        }

        public void SetCharacter(string characterId) => 
            _data.CharacterId = characterId;

        private void OnUnlock(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnUnlock;
            OnUnlocked?.Invoke(this);
        }
    }
}