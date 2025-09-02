using System.Collections.Generic;
using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Enemy;
using PleasantlyGames.RPG.Runtime.Core.Stats.Extension;
using PleasantlyGames.RPG.Runtime.Core.Stats.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main.Sheet
{
    
    public class MainModeSheet : Sheet<int, MainModeSheet.Row>
    {
        [Preserve] public MainModeSheet() { }
        
        public class Row : SheetRowArray<int, Elem>
        {
            [Preserve] public string Complexity { get; private set; }

            [Preserve] public Row() { }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);
                
                if(SheetExt.IsValidationNeeded)
                    Validate();
            }

            private void Validate()
            {
            }

            public int GetMaxLevel()
            {
                var maxLevel = 0;
                foreach (var elem in this)
                    maxLevel = Mathf.Max(maxLevel, elem.Level);
                return maxLevel;
            }
        }
        
        public class Elem : SheetRowElem
        {
            [Preserve] public int Level { get; private set; }
            [Preserve] public string WaveJSON { get; private set; }
            [Preserve] public string BossUnitId { get; private set; }
            [Preserve] public string BossManualStatsJSON { get; private set; }
            [Preserve] public string LocationId { get; private set; }

            private List<List<EnemyData>> _waves;
            public List<List<EnemyData>> Waves => _waves ??= DeserializeWave();

            private List<ManualStatData<UnitStatType>> _bossManualStats;
            public List<ManualStatData<UnitStatType>> BossManualStats => _bossManualStats ??= StatExtension.DeserializeManualStats<UnitStatType>(BossManualStatsJSON);

            [Preserve] public Elem() { }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);
                
                if(SheetExt.IsValidationNeeded)
                    Validate();
            }

            private void Validate()
            {
                _waves = DeserializeWave();
                _bossManualStats = StatExtension.DeserializeManualStats<UnitStatType>(BossManualStatsJSON);
                SheetExt.CheckAsset(LocationId);
                //TODO CHECK CHARACTERS
                // SheetExt.CheckAsset(BossUnitId);
                // foreach (var wave in _waves)
                // foreach (var enemyData in wave)
                //     SheetExt.CheckAsset(enemyData.UnitId);
            }
            
            private List<List<EnemyData>> DeserializeWave()
            {
                if (WaveJSON.IsNullOrWhitespace())
                {
                    Debug.LogError("WaveJSON is null or whitespace");
                    return null;
                }

                List<List<EnemyData>> waveDataList = new List<List<EnemyData>>();
                var waves = JsonConvertLog.DeserializeObject<List<Dictionary<string, int>>>(WaveJSON);
                foreach (var wave in waves)
                {
                    var newWave = new List<EnemyData>();
                    waveDataList.Add(newWave);
                    foreach (var wavePair in wave) 
                        newWave.Add(new EnemyData(wavePair.Key, wavePair.Value));
                }
                
                return waveDataList;
            }
        }
    }
}