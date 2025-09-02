using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Products.Type;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Rewards.Items
{
    public class ItemProductReward : ProductReward
    {
        private readonly Item _item;
        private readonly ItemFacade _itemFacade;
        private readonly ItemProductRewardData _data;

        public override ProductItemType Type => ProductItemType.Item;
        public override Sprite Sprite => _item.Sprite;
        public override string Name { get; }

        public ItemProductReward(ItemFacade itemFacade, Item item,
            ItemProductRewardData data, Color backColor,
            ProductElem config, IUnlockable unlockable) : base(backColor, config, unlockable)
        {
            _itemFacade = itemFacade;
            _item = item;
            _data = data;
            Name = $"X{_data.Amount}";
        }

        public override void Apply() => 
            _itemFacade.AddAmount(_item.Id, _data.Amount);

        public override object GetCertainProductType()
        {
            return _item.Type;
        }
    }
}