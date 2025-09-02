using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.Formula.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Units.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Save;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model
{
    public class UnitStatService
    {
        [Inject] private BalanceContainer _balance;
        [Inject] private ISpriteProvider _spriteProvider;
        [Inject] private UnitStatDataProvider _dataProvider;
        
        private UnitsSheet _unitsSheet;
        private PlayerStatsSheet _playerStatsSheet;
        
        private UnitStatDataContainer _data;
        private readonly List<ImprovableUnitStat> _playerStats = new();
        private readonly Dictionary<(string unitId, UnitStatType type), BaseValueFormula> _statFormulaCache = new ();
        private ExtendedManualFormulaSheet<UnitsSheet, string, UnitStatType> _manualStatsSheet;

        [Preserve]
        public UnitStatService() { }

        public void Initialize()
        {
            _unitsSheet = _balance.Get<UnitsSheet>();
            _playerStatsSheet = _balance.Get<PlayerStatsSheet>();
            _manualStatsSheet = _balance.Get<ExtendedManualFormulaSheet<UnitsSheet, string, UnitStatType>>();
            
            _data = _dataProvider.GetData();
            CreatePlayerStats();
        }

        public IReadOnlyList<ImprovableUnitStat> GetPlayerStats() => 
            _playerStats;

        public ImprovableUnitStat GetPlayerStat(UnitStatType statType)
        {
            foreach (var stat in _playerStats)
                if (stat.Type == statType)
                    return stat;

            return null;
        }

        private void CreatePlayerStats()
        {
            var atlasName = Asset.MainAtlas;
            foreach (var statData in _data.Stats)
            {
                var row = _playerStatsSheet[statData.Type];
                _playerStats.Add(new ImprovableUnitStat(statData, row.Config, _spriteProvider.GetSprite(atlasName, row.ImageName)));
            }
        }
        
        public List<UnitStat> CreateRuntimeStats(string unitId)
        {
            var unitRow = GetStatsConfig(unitId);
            var stats = new List<UnitStat>(unitRow.Count);
            var atlasName = Asset.MainAtlas;
            foreach (var row in unitRow)
            {
                BaseValueFormula valueFormula = null;
                if (row.Config.ValueFormulaType == FormulaType.CustomSheet) 
                    valueFormula = _manualStatsSheet.GetValueFormula(unitId, row.Config.StatType);
                stats.Add(new UnitStat(new StatData(row.StatType, 1), row.Config, _spriteProvider.GetSprite(atlasName, row.ImageName), valueFormula));
            } 
            
            return stats;
        }

        public BigDouble.Runtime.BigDouble GetValue(string unitId, UnitStatType type, int level)
        {
            var unitConfig = GetStatsConfig(unitId);
            var key = (unitId, type);
            if (_statFormulaCache.TryGetValue(key, out var formula))
                return formula.CalculateBigDouble(level);

            foreach (var config in unitConfig)
            {
                if (config.StatType != type) continue;
                var valueFormula = config.ValueFormulaType.CreateFormula(config.ValueFormulaJSON);
                _statFormulaCache.Add(key, valueFormula);
                return valueFormula.CalculateBigDouble(level);
            }

            return 0;
        }

        private UnitsSheet.Row GetStatsConfig(string unitId)
        {
            var unitConfig = _unitsSheet[unitId];
            return unitConfig;
        }
    }
}