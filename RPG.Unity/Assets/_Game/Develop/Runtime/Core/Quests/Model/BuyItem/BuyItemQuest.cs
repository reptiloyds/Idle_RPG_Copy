using System.Collections.Generic;
using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model;
using PleasantlyGames.RPG.Runtime.Core.Quests.Sheet;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Quests.Model.BuyItem
{
    public class BuyItemQuest : Quest
    {
        private readonly IReadOnlyDictionary<ItemType, int> _statistic;
        [Inject] private LootboxService _lootboxService;
        
        private readonly QBuyItemData _data;
        
        public BuyItemQuest(IReadOnlyDictionary<ItemType, int> statistic, QuestRow config, int progress) : base(config, progress)
        {
            _statistic = statistic;
            _data = JsonConvert.DeserializeObject<QBuyItemData>(config.DataJSON);
        }

        public override void Initialize()
        {
            base.Initialize();
            Description += $": {Translator.Translate(_data.Type.ToString())}";
            _statistic.TryGetValue(_data.Type, out var value);
            Progress = value;
            _lootboxService.OnItemsApplied += OnItemsApplied;
            if (Progress >= _data.Amount && !IsComplete)
                Complete();
        }
        
        public override void Dispose()
        {
            base.Dispose();
            _lootboxService.OnItemsApplied -= OnItemsApplied;
        }

        private void OnItemsApplied(Lootbox lootbox, IReadOnlyList<Item> items)
        {
            if(lootbox.ItemType != _data.Type) return;
            
            Progress += items.Count;
            if (Progress >= _data.Amount && !IsComplete)
                Complete();
        }

        public override string GetDescription() => 
            Description;

        public override (float progress, string progressText) GetProgress() => 
            ((float)Progress / _data.Amount, $"{Progress.ToString()}/{_data.Amount.ToString()}");
    }
}