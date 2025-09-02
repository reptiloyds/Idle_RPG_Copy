using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Sheet;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Rewards.Items
{
    public class ItemPeriodicReward : PeriodicReward
    {
        private readonly ItemFacade _itemFacade;
        private readonly ItemPeriodicData _data;
        
        private Item _item;

        public ItemPeriodicReward(PeriodicRewardSheet.Elem config, ISpriteProvider spriteProvider, ITranslator translator, ItemFacade itemFacade)
            : base(config, spriteProvider, translator)
        {
            _itemFacade = itemFacade;
            _data = JsonConvert.DeserializeObject<ItemPeriodicData>(config.RewardJSON);
        }

        public override void Initialize()
        {
            _item = _itemFacade.GetItem(_data.Id);
            base.Initialize();
            Text = $"X{_data.Amount}";
            TypeText = Translator.Translate(_item.Type.ToString());
        }

        public override void Apply(Vector3 viewWorldPosition)
        {
            if(_item == null) return;
            _itemFacade.AddAmount(_item.Id, _data.Amount);
        }

        protected override Sprite GetImageInternal() => 
            _item?.Sprite;
    }
}