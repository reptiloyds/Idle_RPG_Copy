using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons
{
    public struct DungeonRewardViewData
    {
        public readonly Sprite Sprite;
        public readonly double Amount;
        public readonly Color BackColor;

        public DungeonRewardViewData(Sprite sprite, double amount, Color backColor)
        {
            Sprite = sprite;
            Amount = amount;
            BackColor = backColor;
        }
    }
}