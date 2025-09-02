using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Save;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.Model.BuyItem
{
    public class BuyItemDaily : Daily
    {
        private readonly LootboxService _lootboxService;
        private readonly DBuyItemData _configData;

        public BuyItemDaily(DailyRow config, DailyData data, ResourceService resourceService, ITranslator translator, LootboxService lootboxService) : base(config, data, resourceService, translator)
        {
            _lootboxService = lootboxService;
            _configData = JsonConvert.DeserializeObject<DBuyItemData>(config.DataJSON);
        }

        public override void Initialize()
        {
            base.Initialize();
            
            if(!IsComplete)
                _lootboxService.OnItemsApplied += OnItemsApplied;

            if(_configData.Type != ItemType.None)
                Description += Translator.Translate(_configData.Type.ToString());
        }

        public override void Dispose()
        {
            base.Dispose();
            _lootboxService.OnItemsApplied -= OnItemsApplied;
        }

        private void OnItemsApplied(Lootbox lootbox, IReadOnlyList<Item> items)
        {
            if(_configData.Type != ItemType.None && lootbox.ItemType != _configData.Type) return;
            Progress += items.Count;
        }

        protected override int GetTargetValue() => 
            _configData.Amount;

        protected override void OnComplete()
        {
            _lootboxService.OnItemsApplied -= OnItemsApplied;
            base.OnComplete();
        }
    }
}