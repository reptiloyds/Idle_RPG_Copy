using System.Collections.Generic;
using Cathei.BakingSheet;
using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Lootboxes.Sheet
{
    public class LootboxSheet : Sheet<string, LootboxRow>
    {
        [Preserve] public LootboxSheet() { }
    }

    public class LootboxRow : SheetRowArray<string, LootboxElem>
    {
        [Preserve] public ItemType ItemType { get; private set; }
        [Preserve] public string ResourcePurchaseJSON { get; private set; }
        [Preserve] public string BonusPurchaseJSON { get; private set; }
        [Preserve] public string HexColor { get; private set; }
        [Preserve] public string SpriteList { get; private set; }

        private Color _color;
        public Color Color
        {
            get
            {
                if (_color == default) 
                    ColorUtility.TryParseHtmlString(HexColor, out _color);
                return _color;
            }
        }

        private List<string> _sprites;
        public List<string> Sprites => _sprites ??= DeserializeSprites();

        private List<LootboxResourcePurchaseData> _resourcePurchases;
        public List<LootboxResourcePurchaseData> ResourcePurchases => _resourcePurchases ??= DeserializePurchases<LootboxResourcePurchaseData>(ResourcePurchaseJSON);

        private LootboxBonusOpenData _bonusOpen;
        public LootboxBonusOpenData BonusOpen => _bonusOpen ??= DeserializeLootboxPurchaseData();
        
        [Preserve] public LootboxRow() { }

        public override void PostLoad(SheetConvertingContext context)
        {
            base.PostLoad(context);
            
            if(SheetExt.IsValidationNeeded)
                Validate();
        }

        private void Validate()
        {
            _resourcePurchases = DeserializePurchases<LootboxResourcePurchaseData>(ResourcePurchaseJSON);
            _bonusOpen = DeserializeLootboxPurchaseData();
            var sprites = DeserializeSprites();
            string atlasName = null;
            switch (ItemType)
            {
                case ItemType.Stuff:
                    atlasName = Asset.StuffAtlas;
                    break;
                case ItemType.Companion:
                    atlasName = Asset.CompanionAtlas;
                    break;
                case ItemType.Skill:
                    atlasName = Asset.SkillAtlas;
                    break;
            }
            foreach (var sprite in sprites) 
                SheetExt.CheckSprite(atlasName, sprite);
        }

        private LootboxBonusOpenData DeserializeLootboxPurchaseData()
        {
            if (string.IsNullOrWhiteSpace(BonusPurchaseJSON))
                return null;

            return JsonConvertLog.DeserializeObject<LootboxBonusOpenData>(BonusPurchaseJSON);
        }

        private List<T> DeserializePurchases<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<T>(0);

            return JsonConvertLog.DeserializeObject<List<T>>(json);
        }

        private List<string> DeserializeSprites() => 
            JsonConvert.DeserializeObject<List<string>>(SpriteList);
    }

    public class LootboxElem : SheetRowElem
    {
        [Preserve] public int ExpToLevelUp { get; private set; }
        [Preserve] public string ChanceJSON { get; private set; }

        private Dictionary<ItemRarityType, float> _chances;
        public Dictionary<ItemRarityType, float> Chances => _chances ??= DeserializeChances();
        
        [Preserve] public LootboxElem() { }

        public override void PostLoad(SheetConvertingContext context)
        {
            base.PostLoad(context);

            if(SheetExt.IsValidationNeeded)
                Validate();
        }

        private void Validate()
        {
            _chances = DeserializeChances();
        }

        private Dictionary<ItemRarityType, float> DeserializeChances()
        {
            if (string.IsNullOrWhiteSpace(ChanceJSON))
            {
                Debug.LogError("ChanceJSON is empty");
                return new Dictionary<ItemRarityType, float>();
            }

            return JsonConvert.DeserializeObject<Dictionary<ItemRarityType, float>>(ChanceJSON);
        }
    }
}