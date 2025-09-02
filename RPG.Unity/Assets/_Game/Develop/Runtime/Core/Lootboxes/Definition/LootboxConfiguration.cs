using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Sheet;

namespace PleasantlyGames.RPG.Runtime.Core.Lootboxes.Definition
{
    [Serializable]
    public class LootboxConfiguration
    {
        public List<LootboxFtue> Ftues;

        public LootboxFtue GetFTUE(string dataId, int openNumber)
        {
            foreach (var ftue in Ftues)
            {
                if (string.Equals(ftue.LootboxId, dataId) && ftue.OpenNumber == openNumber)
                    return ftue;
            }

            return null;
        }
    }
}