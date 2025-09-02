using Newtonsoft.Json;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Data
{
    public class PiggyBankBonusData
    {
        [JsonProperty("Id")] public string Id;
        [JsonProperty("IsCollected")] public bool IsCollected;
    }
}