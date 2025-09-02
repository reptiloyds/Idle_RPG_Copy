using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.VIP.Model
{
    public class VipBonusData
    {
        public Sprite Sprite { get; }
        public string Label { get; }
        public string Definition { get; }

        public VipBonusData(Sprite sprite, string label, string definition)
        {
            Sprite = sprite;
            Label = label;
            Definition = definition;
        }
    }
}