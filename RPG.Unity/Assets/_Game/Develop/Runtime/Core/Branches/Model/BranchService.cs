using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Branches.Save;
using PleasantlyGames.RPG.Runtime.Core.Branches.Sheet;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Branches.Model
{
    public class BranchService : IDisposable
    {
        [Inject] private ContentService _contentService;
        [Inject] private BalanceContainer _balance;
        [Inject] private BranchDataProvider _dataProvider;
        [Inject] private ISpriteProvider _spriteProvider;
        
        private BranchDataContainer _data;
        private Branch _selectedBranch;

        private readonly List<Branch> _branches = new();

        public IReadOnlyList<Branch> Branches => _branches;
        
        public event Action<Branch> BranchUnlock;
        public event Action SwitchBranch;

        [Preserve]
        public BranchService() { }

        public void Initialize()
        {
            _data = _dataProvider.GetData();
            CreateModels();
        }

        public void Dispose()
        {
            foreach (var branch in _branches) 
                branch.OnUnlocked -= OnBranchUnlocked;
        }

        private void CreateModels()
        {
            var sheet = _balance.Get<BranchSheet>();
            foreach (var data in _data.List)
            {
                var config = sheet[data.Id];
                var sprite = _spriteProvider.GetSprite(config.ImageName);
                var branch = new Branch(data, config, sprite, _contentService.GetBranch(data.Id));
                branch.OnUnlocked += OnBranchUnlocked;
                _branches.Add(branch);
                if (_selectedBranch == null && _data.SelectedBranchId == branch.Id) 
                    _selectedBranch = branch;
            }
        }

        private void OnBranchUnlocked(IUnlockable unlockable) => 
            BranchUnlock?.Invoke((Branch)unlockable);
        
        public Branch GetBranchByCharacter(string characterId)
        {
            foreach (var branch in _branches)
                if (string.Equals(branch.CharacterId, characterId))
                    return branch;

            return null;
        }

        public Branch GetBranch(string branchId)
        {
            foreach (var branch in _branches)
            {
                if(branch.Id != branchId) continue;
                return branch;
            }
            return null;
        }

        public Branch GetSelectedBranch() => 
            _selectedBranch;

        public int GetSelectedBranchIndex() => 
            _branches.IndexOf(_selectedBranch);

        public int UnlockedBranchesCount()
        {
            var result = 0;
            foreach (var branch in _branches)
                if (branch.IsUnlocked) result++;
            return result;
        }

        public void ChangeBranch(Branch branch)
        {
            if(_selectedBranch == branch) return;
            _selectedBranch = branch;
            _data.SelectedBranchId = branch.Id;
            SwitchBranch?.Invoke();
        }

        public void SetCharacterToSelectedBranch(string characterId) => 
            _selectedBranch.SetCharacter(characterId);

        public bool IsDefaultCharacter(string characterId)
        {
            foreach (var branch in _branches)
                if (string.Equals(branch.DefaultCharacterId, characterId)) return true;

            return false;
        }
    }
}