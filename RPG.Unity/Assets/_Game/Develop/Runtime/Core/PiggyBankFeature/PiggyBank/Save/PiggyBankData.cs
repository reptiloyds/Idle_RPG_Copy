using System;
using Newtonsoft.Json;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.PiggyBank.Save
{
    [Serializable]
    public class PiggyBankData
    {
        [JsonProperty("PiggyBankLevel")] public int Level;
        [JsonProperty("CurrentHard")] public int CurrentHard;
        [JsonProperty("CompletedLevelCount")] public int CompletedLevelCount;
        [JsonProperty("StatImprovedCount")] public int StatImprovedCount;
    }
}