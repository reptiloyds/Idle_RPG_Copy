using System;
using System.Collections.Generic;
using System.Linq;
using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Analytics.Types;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Products.Extensions;
using PleasantlyGames.RPG.Runtime.Core.Products.Model.Periodic;
using PleasantlyGames.RPG.Runtime.Core.Products.Type;
using UnityEngine;
using UnityEngine.Scripting;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Sheet
{
    public class ProductsSheet : Sheet<string, ProductRow>
    {
        [Preserve]
        public ProductsSheet()
        {
        }

        public override void PostLoad(SheetConvertingContext context)
        {
            base.PostLoad(context);

            if (SheetExt.IsValidationNeeded) 
                Validate();
        }

        private void Validate()
        {
            Dictionary<PeriodicType, List<int>> orders = new();
            foreach (var value in Enum.GetValues(typeof(PeriodicType)))
            {
                var type = (PeriodicType)value;
                if(type == PeriodicType.None) continue;
                orders.Add(type, new List<int>());
            }
            foreach (var row in this)
            {
                if(row.Placement != ProductPlacement.Periodic) continue;
                if (!orders[row.PeriodicData.Type].Contains(row.PeriodicData.Order)) 
                    orders[row.PeriodicData.Type].Add(row.PeriodicData.Order);
            }

            foreach (var kvp in orders)
            {
                kvp.Value.Sort();
                int? prev = null;

                foreach (var number in kvp.Value)
                {
                    if (prev.HasValue && number - prev > 1) 
                        Logger.LogError($"Periodic products with type {kvp.Key} has a gap {prev} - {number}");
                    prev = number;
                }
            }
            
        }
    }

    public class ProductRow : SheetRowArray<string, ProductElem>
    {
        [Preserve] public int NutakuId { get; private set; }
        [Preserve] public ProductPlacement Placement { get; private set; }
        [Preserve] public string PlacementData { get; private set; }
        [Preserve] public Consumation Consumation { get; private set; }
        [Preserve] public string NameLocToken { get; private set; }
        [Preserve] public string DescriptionLocToken { get; private set; }
        [Preserve] public string ImageUrl { get; private set; }
        [Preserve] public ProductPriceType PriceType { get; private set; }
        [Preserve] public PurchaseCooldownType PurchaseCooldown { get; private set; }
        [Preserve] public int PurchaseLimit { get; private set; }
        [Preserve] public int LocalPrice { get; private set; }
        [Preserve] public string LocalCurrency { get; private set; }
        [Preserve] public ProductVisualType Visual { get; private set; }
        [Preserve] public AnalyticsItemType AnalyticsItemType { get; private set; }
        
        private PeriodicProductData _periodicData;
        public PeriodicProductData PeriodicData => _periodicData ??= Placement == ProductPlacement.Periodic ?
            JsonConvertLog.DeserializeObject<PeriodicProductData>(PlacementData) : null;


        [Preserve]
        public ProductRow()
        {
        }

        public override void PostLoad(SheetConvertingContext context)
        {
            base.PostLoad(context);

            if (SheetExt.IsValidationNeeded)
                Validate();
        }

        private void Validate()
        {
            var mergeKeysAmount = this.Count(item => !string.IsNullOrEmpty(item.MergeKey));
            if (mergeKeysAmount > 1) 
                Logger.LogError("Only one item can have MergeKey");

            switch (Placement)
            {
                case ProductPlacement.Periodic:
                    if(PeriodicData == null)
                        Logger.LogError("Invalid periodic type");
                    else
                    {
                        if(PeriodicData.Type == PeriodicType.None)
                            Logger.LogError("PeriodicType should not be equals to None");
                        if (PeriodicData.Order <= 0)
                            Logger.LogError("Order should not be less or equal to 0");
                    }
                    break;
            }
            
            if (AnalyticsItemType == AnalyticsItemType.None)
                Logger.LogError("Analytics item type should not be None");
        }
    }

    public class ProductElem : SheetRowElem
    {
        [Preserve] public string MergeKey { get; private set; }
        [Preserve] public bool IsBonus { get; private set; }
        [Preserve] public ProductItemType ItemType { get; private set; }
        [Preserve] public string ItemJSON { get; private set; }
        [Preserve] public string Color { get; private set; }

        private Color _backColor;

        public Color BackColor
        {
            get
            {
                if (_backColor == default) DeserializeColor(out _backColor);
                return _backColor;
            }
        }

        [Preserve]
        public ProductElem()
        {
        }

        public override void PostLoad(SheetConvertingContext context)
        {
            base.PostLoad(context);

            if (!Application.isPlaying)
                Validate();
        }

        private void Validate()
        {
            ItemType.TryDeserialize(ItemJSON);
            DeserializeColor(out _backColor);
        }

        private void DeserializeColor(out Color color) =>
            ColorUtility.TryParseHtmlString(Color, out color);
    }
}