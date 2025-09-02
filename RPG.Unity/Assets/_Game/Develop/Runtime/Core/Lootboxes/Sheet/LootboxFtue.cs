using System;
using System.Collections.Generic;

namespace PleasantlyGames.RPG.Runtime.Core.Lootboxes.Sheet
{
    [Serializable]
    public class LootboxFtue
    {
        public string LootboxId;
        public int OpenNumber;
        public List<string> ItemIds;
    }
}