using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PleasantlyGames.RPG.Runtime.Analytics.Save
{
    [Serializable]
    public class AnalyticsData
    {
        [JsonProperty("TutorialId")] public string TutorialId;
        [JsonProperty("TutorialStep")] public int TutorialStep;
        [JsonProperty("TutorialTimer")] public float TutorialTimer;
        [JsonProperty("StepTimer")] public float StepTimer;
        [JsonProperty("PiggyBankMaxLevel")] public int PiggyBankMaxLevel;
        [JsonProperty("FirstPurchaseFlag")] public bool IsFirstPurchaseRequested;
        [JsonProperty("PurchaseOrder")] public int PurchaseOrder;
        [JsonProperty("LastPurchaseDate")] public DateTime LastPurchaseDate;
        [JsonProperty("TotalPrice")] public decimal TotalPrice;
        [JsonProperty("ShownPeriodicProducts")] public List<string> PeriodicOfferShownList = new();
        [JsonProperty("BoughtPeriodicProducts")] public List<string> PeriodicOfferBoughtList = new();
    }
}