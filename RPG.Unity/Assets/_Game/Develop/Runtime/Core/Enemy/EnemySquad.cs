using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Squad;
using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Enemy
{
    public class EnemySquad : MortalSquad
    {
        private UnitView _bossUnitView;

        public override TeamType TeamType => TeamType.Enemy;

        [Preserve]
        public EnemySquad()
        {
        }

        public async UniTask SpawnWaveAsync(List<EnemyData> wave, int level,
            IReadOnlyDictionary<UnitStatType, StatModifier> statModifiers = null)
        {
            await SpawnUnits(wave, level);
            if (statModifiers == null) return;
            foreach (var kvp in statModifiers)
                AddModifier(kvp.Key, kvp.Value);
        }

        public async UniTask<UnitView> SpawnBossAsync(string unitId, Vector2 healthBarSize, int level,
            List<ManualStatData<UnitStatType>> manualStats = null,
            IReadOnlyDictionary<UnitStatType, StatModifier> statModifiers = null)
        {
            var spawnPoint = SpawnProvider.GetMiddleSpawnPoint();
            _bossUnitView = await SpawnUnitAsync(unitId, spawnPoint);
            var stats = StatService.CreateRuntimeStats(_bossUnitView.Id);

            if (manualStats != null)
            {
                foreach (var manualStat in manualStats)
                foreach (var stat in stats)
                {
                    if (stat.Type != manualStat.Type) continue;
                    stat.SetValueFormula(
                        new BaseValueFormula(new BigDouble.Runtime.BigDouble(manualStat.M, manualStat.E)));
                    break;
                }
            }

            _bossUnitView.SetStats(stats);
            _bossUnitView.Initialize();

            SetStatsLevel(_bossUnitView, level);

            AppendUnit(_bossUnitView);
            _bossUnitView.Health.Bar.EnableValueView();
            _bossUnitView.Health.Bar.ChangeSize(healthBarSize);

            if (statModifiers != null)
            {
                foreach (var kvp in statModifiers)
                    AddBossModifier(kvp.Key, kvp.Value);
            }

            return _bossUnitView;
        }

        private void AddBossModifier(UnitStatType statType, StatModifier statModifier) =>
            ApplyModifier(_bossUnitView, statType, statModifier);

        private async UniTask SpawnUnits(List<EnemyData> wave, int level)
        {
            foreach (var waveData in wave)
            {
                for (var i = 0; i < waveData.Count; i++)
                {
                    var spawnPoint = SpawnProvider.GetRandomFreePoint();
                    var unit = await SpawnUnitAsync(waveData.UnitId, spawnPoint);
                    var stats = StatService.CreateRuntimeStats(unit.Id);
                    unit.SetStats(stats);
                    unit.Initialize();

                    SetStatsLevel(unit, level);
                    AppendUnit(unit);
                }
            }
        }

        private void SetStatsLevel(UnitView unitView, int level)
        {
            if (level <= 1) return;
            var stats = unitView.GetStats();
            foreach (var stat in stats)
                stat.SetLevel(level);
        }
    }
}