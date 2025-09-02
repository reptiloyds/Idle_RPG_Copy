using System;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons
{
    [Serializable]
    public class SubModeSetup
    {
        public AssetReferenceTexture2D BackgroundRef;
        public Color Color;
        public ResourceType EnterPriceType;
        [MinValue(0)] public int DailyEnterResourceAmount = 2;
        [MinValue(0)] public int DailyEnterBonusAmount = 2;
    }
}