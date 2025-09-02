using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Quests.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Model;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Quests.Model.EnhanceStat
{
    public class EnhanceStatQuest : Quest
    {
        private readonly IReadOnlyDictionary<UnitStatType, int> _statistic;
        [Inject] private StatImprover _statImprover;
        
        private readonly QEnhanceStatData _data;
        
        public EnhanceStatQuest(IReadOnlyDictionary<UnitStatType, int> statistic, QuestRow config, int progress) : base(config, progress)
        {
            _statistic = statistic;
            _data = JsonConvert.DeserializeObject<QEnhanceStatData>(config.DataJSON);
        }

        public override void Initialize()
        {
            base.Initialize();
            Description += $": {Translator.Translate(_data.Type.ToString())}";
            _statistic.TryGetValue(_data.Type, out var value);
            Progress = value;
            _statImprover.OnLevelUpStat += OnLevelUpStat;
            if (Progress >= _data.Amount && !IsComplete)
                Complete();
        }

        public override void Dispose()
        {
            base.Dispose();
            _statImprover.OnLevelUpStat -= OnLevelUpStat;
        }

        private void OnLevelUpStat(UnitStatType statType)
        {
            if(statType != _data.Type) return;
            
            Progress++;
            if (Progress >= _data.Amount && !IsComplete)
                Complete();
        }

        public override string GetDescription() => 
            Description;

        public override (float progress, string progressText) GetProgress()
        {
            return ((float)Progress / _data.Amount, $"{Progress.ToString()}/{_data.Amount.ToString()}");
        }
    }
}