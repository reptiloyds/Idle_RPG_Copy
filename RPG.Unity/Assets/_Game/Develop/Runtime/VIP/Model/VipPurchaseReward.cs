using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.VIP.Model
{
    public class VipPurchaseReward
    {
        public Color Color { get; }
        public Sprite Sprite { get; }
        public string Value { get; }

        public VipPurchaseReward(Color color, Sprite sprite, string value)
        {
            Color = color;
            Sprite = sprite;
            Value = value;
        }
    }
}