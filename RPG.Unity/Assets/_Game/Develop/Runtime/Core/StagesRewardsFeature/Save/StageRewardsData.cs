using System;
using Newtonsoft.Json;

namespace PleasantlyGames.RPG.Runtime.Core.StagesRewardsFeature.Save
{
    [Serializable]
    public class StageRewardsData
    {
        [JsonProperty("CurrentIndex")] public int CurrentIndex;
    }
}