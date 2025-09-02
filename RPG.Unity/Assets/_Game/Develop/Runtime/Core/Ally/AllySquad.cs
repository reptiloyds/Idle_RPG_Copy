using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement.Forecast;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Core.Location.Model;
using PleasantlyGames.RPG.Runtime.Core.Squad;
using PleasantlyGames.RPG.Runtime.Core.Units.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Ally
{
    public class AllySquad : MortalSquad, IAssetUser
    {
        [Inject] private BranchService _branchService;
        [Inject] private CharacterService _characterService;
        [Inject] private LocationUnitCollection _locationUnitCollection;
        
        private readonly List<(Branch branch, UnitView unit)> _unitTuples = new();

        public override TeamType TeamType => TeamType.Ally;
        
        [Preserve]
        public AllySquad()
        {
        }
        
        public override void Initialize()
        {
            base.Initialize();

            _characterService.OnAnySwitched += CharacterAnySwitched;
            _characterService.OnEvolved += OnEvolved;
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _characterService.OnAnySwitched -= CharacterAnySwitched;
            _characterService.OnEvolved -= OnEvolved;
        }

        private void OnEvolved(Character character)
        {
            var branch = _branchService.GetBranchByCharacter(character.Id);
            if(branch == null) return;
            
            RefreshCharacterForBranch(branch);
        }

        private void CharacterAnySwitched() => 
            RefreshCharacterForBranch(_branchService.GetSelectedBranch());

        private async void RefreshCharacterForBranch(Branch branch)
        {
            var unit = GetUnit(branch);
            if(unit == null) return;
            RemoveUnit(unit);
            await SpawnAsync(branch);
        }

        public UnitView GetUnit(Branch branch)
        {
            foreach (var tuple in _unitTuples)
            {
                if(tuple.branch != branch) continue;
                return tuple.unit;
            }

            return null;
        }

        public async UniTask SpawnUnitsAsync()
        {
            var spawnTasks = _branchService.Branches
                .Where(branch => branch.IsUnlocked)
                .Select(SpawnAsync)
                .ToArray();

            await UniTask.WhenAll(spawnTasks);
        }
        
        private async UniTask SpawnAsync(Branch branch)
        {
            var spawnPoint = SpawnProvider.GetFreePoint();
            var character = _characterService.GetCharacter(branch.CharacterId);
            
            var unit = await SpawnUnitAsync(character.UnitId, spawnPoint, character.Evolution);
            var stats = StatService.GetPlayerStats();
            unit.SetStats(stats);
            unit.Initialize();
            
            _unitTuples.Add((branch, unit));
            AppendUnit(unit);
        }

        protected override void AppendUnit(UnitView unitView)
        {
            base.AppendUnit(unitView);
            
            // _locationUnitCollection.AppendUnit(unitView);
        }

        protected override void RemoveUnit(UnitView unitView)
        {
            // _locationUnitCollection.RemoveUnit(unitView);
            base.RemoveUnit(unitView);

            foreach (var tuple in _unitTuples)
            {
                if(tuple.unit != unitView) continue;
                _unitTuples.Remove(tuple);
                break;
            }
        }

        public event Action OnNeedsChanged;
        
        public void FillNeeds(in Dictionary<AssetType, HashSet<string>> needs)
        {
            var unitSheet = Balance.Get<UnitsSheet>();
            var unitsHashSet = needs[AssetType.Unit];
            foreach (var branch in _branchService.Branches)
            {
                if(!branch.IsUnlocked) continue;
                var character = _characterService.GetCharacter(branch.CharacterId);
                unitsHashSet.Add(unitSheet[character.UnitId].GetPrefabId(character.Evolution));
            } 
        }
    }
}