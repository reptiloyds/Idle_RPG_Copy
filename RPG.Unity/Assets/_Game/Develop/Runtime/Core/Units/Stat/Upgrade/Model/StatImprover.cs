using System;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Audio.Model;
using PleasantlyGames.RPG.Runtime.Core.Ally;
using PleasantlyGames.RPG.Runtime.Core.AudioTypes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.Effects.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Save;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Unlock;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Unlock.Base;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Model
{
    public class StatImprover
    {
        [Inject] private StatImproverDataProvider _dataProvider;
        
        [Inject] private UnitStatService _statService;
        [Inject] private ITranslator _translator;
        [Inject] private AllySquad _allySquad;
        [Inject] private EffectFactory _effectFactory;
        [Inject] private PowerCalculator _powerCalculator;
        [Inject] private BranchService _branchService;
        [Inject] private IAudioService _audioService;

        private List<ImprovableUnitStat> _stats;
        private readonly Dictionary<ImprovableUnitStat, BaseUnlock> _unlockConditions = new();
        private AudioEmitter _audioEmitter;
        private const float _radiusOffset = 0.5f;
        private StatImproverDataContainer _data;

        public IReadOnlyList<ImprovableUnitStat> Stats => _stats;
        public IReadOnlyDictionary<ImprovableUnitStat, BaseUnlock> UnlockConditions => _unlockConditions;
        public IReadOnlyDictionary<UnitStatType, int> ImproveStatistic => _data.ImproveStatistic;
        public event Action<UnitStatType> OnLevelUpStat;

        [Preserve]
        public StatImprover()
        {
        }

        public void Initialize()
        {
            _data = _dataProvider.GetData();
            _audioEmitter = _audioService
                .CreateLocalSound(SFX_Common.SFX_Common_StatUpgrade)
                .DontRelease()
                .Build();
            
            Setup();
        }

        public void UnlockAllCondition()
        {
            var unlockCopy = new Dictionary<ImprovableUnitStat, BaseUnlock>(_unlockConditions);
            foreach (var kvp in unlockCopy)
                OnUnlocked(kvp.Value);
        }

        private void Setup()
        {
            _stats = _statService.GetPlayerStats().Where(item => item.IsUpgradable).ToList();
            _powerCalculator.Setup(_stats);

            foreach (var stat in _stats)
            {
                stat.OnLevelUp += OnStatLevelUp;
                CreateUnlockConditions(stat);   
            }
        }

        public void Purchase(UnitStatType statType)
        {
            var stat = GetStat(statType);
            stat.LevelUp();
            
            _data.ImproveStatistic.TryAdd(statType, 0);
            _data.ImproveStatistic[statType]++;
            OnLevelUpStat?.Invoke(statType);
        }

        private void CreateUnlockConditions(ImprovableUnitStat stat)
        {
            if (stat.IsUnlocked) return;

            var condition = CreateCondition(stat);
            _unlockConditions.Add(stat, condition);

            condition.OnUnlocked += OnUnlocked;
        }

        private void OnUnlocked(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnUnlocked;
            foreach (var kvp in _unlockConditions)
            {
                if (kvp.Value != unlockable) continue;
                kvp.Key.Unlock();
                _unlockConditions.Remove(kvp.Key);
                break;
            }
        }

        private BaseUnlock CreateCondition(ImprovableUnitStat stat)
        {
            BaseUnlock condition;
            switch (stat.UnlockType)
            {
                case StatUnlockType.StatLevel:
                    var statLevelData = JsonConvertLog.DeserializeObject<StatLevelUnlockData>(stat.UnlockJSON);
                    var targetStat = GetStat(statLevelData.Type);
                    condition = new StatLevelUnlock(stat, _translator, targetStat, statLevelData);
                    break;
                default:
                    Debug.LogError("Unknown condition type: " + stat.Type);
                    return null;
            }

            condition.Initialize();
            return condition;
        }

        private void OnStatLevelUp()
        {
            var units = _allySquad.GetUnits();
            UnitView unitView = null;
            if (units.Count > 0) unitView = units[0];
            if(unitView == null || unitView.IsDead) return;
            var effect = _effectFactory.Create(Asset.LevelUpEffect);
            effect.Follow(unitView.Visual.transform, Vector3.down * unitView.Visual.transform.localPosition.y);
            effect.transform.localScale = Vector3.one * (unitView.Radius + _radiusOffset);
            
            _audioEmitter.Play();
        }

        public UnitStat GetStat(UnitStatType type)
        {
            foreach (var stat in _stats)
                if (stat.Type == type)
                    return stat;

            return null;
        }
    }
}