using System;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.SoftRush.Definition
{
    [Serializable]
    public class SoftRushConfiguration
    {
        public Vector2 BossHealthBarSize = new Vector2(220, 45);
        [MinValue(0)] public float AutoLoseTimeInSeconds = 45f;
        public ResourceType RewardType = ResourceType.Soft;
        public Color RewardColor;
        public SubModeSetup SubModeSetup;
    }
}