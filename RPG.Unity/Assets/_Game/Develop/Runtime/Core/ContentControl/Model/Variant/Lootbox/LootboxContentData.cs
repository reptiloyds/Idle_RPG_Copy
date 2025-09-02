using System;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Variant.Lootbox
{
    [Serializable]
    public class LootboxContentData
    {
        public string LootboxId;
        
        [Preserve]
        public LootboxContentData()
        {
        }
    }
}