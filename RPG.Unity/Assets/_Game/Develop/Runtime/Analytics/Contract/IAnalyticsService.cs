using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Analytics.Save;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Analytics.Contract
{
    public interface IAnalyticsService
    {
        UniTask Initialize();
        void OnLog(string condition, string stacktrace, LogType type);
        void SendAnalyticsSdkReady();
        void SendGameStart();
        void SendCoreSceneLoaded();
        void SendBalanceLoadingStatus(bool status);
        void SendProgressLoaded();
        void SendCatalogsUpdated();
        void SendInternetConnectionChecked(bool status);
        void SendLanguage(string getLanguage);
        void SendTutorialStarted(string tutorialId);
        void SendTutorialCompleted(string tutorialId);
        void SendTutorialStepStarted(string tutorialId, int tutorialOrder);
        void SendTutorialStepCompleted(string tutorialId, int tutorialOrder);
        void SetData(AnalyticsData data);
        void SendLevelChanged(int mainModeId, int mainModeLevel);
        void SendLootboxBought(ItemType type, int amount);
        void SendPiggyBankBoughtNotMax(int oldLevel, int newLevel, int hardCollected, int capacity);
        void SendPiggyBankBoughtMax(int oldLevel, int newLevel, int hardCollected, int capacity);
        void SendPurchaseAttempt(Product product);
        void SendIfFirstPurchase(Product product);
        void SendCompletedPurchase(string platformId, Product product, decimal getPrice, string getCurrency);
        void SendLastPurchase();
        void SendPeriodicOfferShown(Product product);
        void SendPurchasePeriodicProduct(Product product);
        public IReadOnlyList<string> GetProductsShowedProducts();
    }
}
