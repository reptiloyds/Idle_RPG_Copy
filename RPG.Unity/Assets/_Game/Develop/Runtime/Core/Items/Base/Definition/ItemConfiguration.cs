using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Base.Definition
{
    [Serializable]
    public class ItemConfiguration
    {
        [SerializeField] private List<ItemRarityData> _itemRarities;

        public void GetColor(ItemRarityType rarityType, out Color color)
        {
            foreach (var data in _itemRarities)
            {
                if(data.Rarity != rarityType) continue;
                color = data.Color;
                return;
            }
            color = Color.white;
        }
    }

    [Serializable]
    public class ItemRarityData
    {
        public ItemRarityType Rarity;
        public Color Color;
    }
}