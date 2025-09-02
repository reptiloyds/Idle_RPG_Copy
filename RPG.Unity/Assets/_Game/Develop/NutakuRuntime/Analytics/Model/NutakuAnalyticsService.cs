using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameAnalyticsSDK;
using GameAnalyticsSDK.Setup;
using PleasantlyGames.RPG.Runtime.Analytics.Contract;
using PleasantlyGames.RPG.Runtime.Analytics.ErrorsAnalytics;
using PleasantlyGames.RPG.Runtime.Analytics.Save;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Type;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.NutakuRuntime.Analytics.Model
{
    public class NutakuAnalyticsService : IAnalyticsService, IInitializable, IDisposable, ITickable
    {
        private AnalyticsData _data;
        private readonly ErrorAnalyticsLogger _errorAnalyticsLogger = new();

        [Inject] private TimeService _timeService;

        [Preserve]
        public NutakuAnalyticsService()
        {

        }

        public UniTask Initialize()
        {
            GameAnalytics.Initialize();
            return UniTask.CompletedTask;
        }

        public void SetData(AnalyticsData data)
        {
            _data = data;
        }

        void IInitializable.Initialize() =>
            Application.logMessageReceived += OnLog;

        void IDisposable.Dispose() =>
            Application.logMessageReceived -= OnLog;

        public void Tick()
        {
            if (_data == null)
                return;

            if (!string.IsNullOrWhiteSpace(_data.TutorialId))
            {
                float delta = Time.unscaledDeltaTime;
                _data.TutorialTimer += delta;

                if (_data.TutorialStep > -1)
                    _data.StepTimer += delta;
            }

        }

        #region BaseEvents

        public void OnLog(string condition, string stacktrace, LogType type)
        {
#if !UNITY_EDITOR
            if (!GameAnalytics.Initialized)
                return;
            
            var message = _errorAnalyticsLogger.GetMessage(condition, stacktrace, type);
            
            if (String.IsNullOrWhiteSpace(message))
                return;

            GAErrorSeverity severity = type switch
            {
                LogType.Error => GAErrorSeverity.Error,
                LogType.Exception => GAErrorSeverity.Critical,
                _ => GAErrorSeverity.Info
            };
            GameAnalytics.NewErrorEvent(severity, message);
#endif
        }

        public void SendAnalyticsSdkReady()
        {
            string version = Settings.VERSION;
            Logger.Log($"sdk: gameanalytics: {version}");
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent($"sdk:gameanalytics:{version}");
#endif
        }

        public void SendGameStart()
        {
            Logger.Log($"game: start_time at {Time.time:F2}");
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent("game:start_time", Time.time);
            GameAnalytics.NewDesignEvent("game:start_flag", 1);
#endif
        }

        public void SendCoreSceneLoaded()
        {
            Logger.Log($"game: core_scene_loaded at {Time.time}");
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent("game:core_scene_loaded", Time.time);
            GameAnalytics.NewDesignEvent("game:core_scene_loaded_flag", 1);
#endif
        }

        public void SendBalanceLoadingStatus(bool status)
        {
            Logger.Log($"sdk: balance loading status: {(status ? "fetched" : "failed")} at {Time.time:F2}");
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent($"game:balance_loading_status:{status}", Time.time);
            GameAnalytics.NewDesignEvent($"game:balance_loading_flag:{status}", 1);
#endif
        }

        public void SendProgressLoaded()
        {
            Logger.Log($"sdk: progress data loaded at {Time.time:F2}");
        
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent($"game:progress_data_loaded", Time.time);
            GameAnalytics.NewDesignEvent("game:progress_loaded_flag", 1);
#endif
        }

        public void SendCatalogsUpdated()
        {
            Logger.Log($"sdk: catalogs updated at {Time.time:F2}");
        
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent($"game:catalogs_updated", Time.time);
            GameAnalytics.NewDesignEvent("game:catalogs_updated_flag", 1);
#endif
        }

        public void SendInternetConnectionChecked(bool status)
        {
            Logger.Log($"game: internet connection is {(status ? "succeeded" : "failed")} at {Time.time:F2}s");
        
#if !UNITY_EDITOR
            string statusLabel = status ? "succeeded" : "failed";
            GameAnalytics.NewDesignEvent($"game:internet_connection:{statusLabel}", Time.time);
            GameAnalytics.NewDesignEvent($"game:internet_connection_flag:{status}", 1);
#endif
        }

        public void SendLanguage(string language)
        {
            string normalized = language.ToLowerInvariant().Replace("_", "-");
            string shortCode = normalized.Split('-')[0]; // en, ru, fr, etc.
            Logger.Log($"game: language is {shortCode}");
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent($"game:language:{shortCode}");
#endif
        }

        public void SendTutorialStarted(string tutorialId)
        {
            if (_data == null)
                return;

            if (!String.IsNullOrWhiteSpace(_data.TutorialId) && _data.TutorialId.Equals(tutorialId))
                return;

            _data.TutorialId = tutorialId;
            _data.TutorialTimer = 0;
            
            Logger.Log($"tutorial: {tutorialId} started");
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent($"tutorial:{tutorialId}:started");
            GameAnalytics.NewDesignEvent($"tutorial:{tutorialId}:started_flag", 1);
#endif
        }

        public void SendTutorialCompleted(string tutorialId)
        {
            if (_data == null)
                return;

            if (_data.TutorialId.Equals(tutorialId))
            {
                Logger.Log($"tutorial: {tutorialId} completed for {_data.TutorialTimer:F2}");
#if !UNITY_EDITOR
                GameAnalytics.NewDesignEvent($"tutorial:{tutorialId}:completed", _data.TutorialTimer);
                GameAnalytics.NewDesignEvent($"tutorial:{tutorialId}:completed_flag", 1);
#endif

                _data.TutorialId = null;
            }
        }

        public void SendTutorialStepStarted(string tutorialId, int tutorialOrder)
        {
            if (_data == null)
                return;

            if (!String.IsNullOrWhiteSpace(_data.TutorialId) && !_data.TutorialId.Equals(tutorialId))
                return;

            if (_data.TutorialStep == tutorialOrder)
                return;

            _data.TutorialStep = tutorialOrder;
            _data.StepTimer = 0;
            
            Logger.Log($"tutorial:steps:started - {tutorialId} -> step {tutorialOrder}");
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent($"tutorial:{tutorialId}:step:{tutorialOrder}:started");
            GameAnalytics.NewDesignEvent($"tutorial:{tutorialId}:step:{tutorialOrder}:started_flag", 1);
#endif
        }

        public void SendTutorialStepCompleted(string tutorialId, int tutorialOrder)
        {
            if (_data == null)
                return;

            if (!_data.TutorialId.Equals(tutorialId) || _data.TutorialStep != tutorialOrder)
                return;
            
            Logger.Log($"tutorial:steps:completed - {tutorialId} -> step {tutorialOrder}, time: {_data.StepTimer:F2}");
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent($"tutorial:{tutorialId}:step:{tutorialOrder}:completed",_data.StepTimer);
            GameAnalytics.NewDesignEvent($"tutorial:{tutorialId}:step:{tutorialOrder}:completed_flag", 1);
#endif
            _data.TutorialStep = -1;
        }

        public void SendLevelChanged(int locationId, int level)
        {
            Logger.Log($"game:level_change:{locationId}-{level}");
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent($"game:level_change:{locationId}-{level}", 1);
#endif
        }

        public void SendLootboxBought(ItemType type, int amount)
        {
            Logger.Log($"game:{type}:{amount} bought");
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent($"game:lootbox:{type}", amount);
#endif
        }

        public void SendPiggyBankBoughtNotMax(int oldLevel, int newLevel, int hardCollected, int capacity)
        {
            Logger.Log($"game:piggy_bank:bought:from_{oldLevel}_to_{newLevel}");
            Logger.Log($"game:piggy_bank:hard_collected = {hardCollected}");
            Logger.Log($"game:piggy_bank:capacity = {capacity}");
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent($"game:piggy_bank:bought:from_{oldLevel}_to_{newLevel}");
            GameAnalytics.NewDesignEvent($"game:piggy_bank:hard_collected", hardCollected);
            GameAnalytics.NewDesignEvent($"game:piggy_bank:capacity", capacity);
#endif
        }

        public void SendPiggyBankBoughtMax(int oldLevel, int newLevel, int hardCollected, int capacity)
        {
            if (_data.PiggyBankMaxLevel != newLevel)
            {
                Logger.Log($"game:piggy_bank:bought:from_{oldLevel}_to_{newLevel}");
                Logger.Log("game:piggy_bank:is_max_level");
                _data.PiggyBankMaxLevel = newLevel;
            }
            
            Logger.Log($"game:piggy_bank:hard_collected = {hardCollected}");
            Logger.Log($"game:piggy_bank:capacity = {capacity}");
#if !UNITY_EDITOR
            if (_data.PiggyBankMaxLevel != newLevel)
            {
                GameAnalytics.NewDesignEvent($"game:piggy_bank:is_max_level", 1);
                GameAnalytics.NewDesignEvent($"game:piggy_bank:bought:from_{oldLevel}_to_{newLevel}");
                _data.PiggyBankMaxLevel = newLevel;
            }
            
            Logger.Log($"game:piggy_bank:hard_collected = {hardCollected}");
            Logger.Log($"game:piggy_bank:capacity = {capacity}");
#endif
        }

        public void SendPurchaseAttempt(Product product)
        {
            Logger.Log($"purchase:try:{product.AnalyticsItemType}:{product.Id}");
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent($"purchase:try:{product.AnalyticsItemType}:{product.Id}", 1);
#endif
        }

        public void SendIfFirstPurchase(Product product)
        {
            if (_data.IsFirstPurchaseRequested)
                return;
            
            _data.IsFirstPurchaseRequested = true;
            
            Logger.Log($"purchase:first_purchase:{product.Id}");
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent($"purchase:first_purchase", 1);
            GameAnalytics.NewDesignEvent($"purchase:first_purchase:{product.Id}", 1);
#endif
        }

        public void SendCompletedPurchase(string platformId, Product product, decimal price, string getCurrency)
        {
            _data.LastPurchaseDate = _timeService.Now();
            _data.PurchaseOrder++;
            _data.TotalPrice += price;
            
            Logger.Log($"purchase:complete:{product.AnalyticsItemType}:{product.Id}");
            Logger.Log($"purchase:price:{product.AnalyticsItemType}:{product.Id}:{price}");
            Logger.Log($"purchase:currency:{getCurrency.ToLowerInvariant()}");
            Logger.Log($"purchase:platform:{platformId}");
            Logger.Log($"user_purchase_meta:orders:{_data.PurchaseOrder}");
            Logger.Log($"user_purchase_meta:total_price:{_data.TotalPrice}");
#if !UNITY_EDITOR
            GameAnalytics.NewDesignEvent($"purchase:complete:{product.AnalyticsItemType}:{product.Id}", 1);
            GameAnalytics.NewDesignEvent($"purchase:price:{product.AnalyticsItemType}:{product.Id}", (float)price);
            GameAnalytics.NewDesignEvent($"purchase:currency:{getCurrency.ToLowerInvariant()}", 1);
            GameAnalytics.NewDesignEvent($"purchase:platform:{platformId}", 1);
            GameAnalytics.NewDesignEvent("user_purchase_meta:orders", _data.PurchaseOrder);
            GameAnalytics.NewDesignEvent("user_purchase_meta:total_price", (float)_data.TotalPrice);
#endif

            if (product.Placement == ProductPlacement.Periodic)
                SendPurchasePeriodicProduct(product);
        }

        public void SendLastPurchase()
        {
            if (_timeService.IsFirstSessionToday && _data.PurchaseOrder > 0)
            {
                int daysFromLastPurchase = _timeService.DaysFrom(_data.LastPurchaseDate);
                if (daysFromLastPurchase > 0)
                {
                    Logger.Log($"game:last_purchase:days_from_last_purchase:{daysFromLastPurchase}");
#if !UNITY_EDITOR
                    GameAnalytics.NewDesignEvent("user_purchase_meta:days_from_last_purchase", daysFromLastPurchase);
                    GameAnalytics.NewDesignEvent($"user_purchase_meta:days_from_last_purchase:bucket:{Bucket(daysFromLastPurchase)}", 1);
#endif
                }
            }
        }

        public void SendPeriodicOfferShown(Product product)
        {
            if (!_data.PeriodicOfferShownList.Contains(product.Id))
            {
                _data.PeriodicOfferShownList.Add(product.Id);
                Logger.Log($"game:periodic_offer:shown:{product.Id}");
#if !UNITY_EDITOR
                GameAnalytics.NewDesignEvent($"game:periodic_offer:shown:{product.Id}", 1);
#endif
            }
        }

        public void SendPurchasePeriodicProduct(Product product)
        {
            if (!_data.PeriodicOfferBoughtList.Contains(product.Id))
            {
                _data.PeriodicOfferBoughtList.Add(product.Id);
                Logger.Log($"game:periodic_offer:bought:{product.Id}");
#if !UNITY_EDITOR
                GameAnalytics.NewDesignEvent($"game:periodic_offer:bought:{product.Id}", 1);
#endif
            }
        }

        public IReadOnlyList<string> GetProductsShowedProducts()
        {
            return _data.PeriodicOfferShownList;
        }

        private string Bucket(int days)
        {
            if (days <= 0) return "0";
            if (days <= 3) return "1-3";
            if (days <= 7) return "4-7";
            if (days <= 30) return "8-30";
            return "30+";
        }

        #endregion

        #region NutakuEvents

        public void SendNutakuInitializationStatus(bool status)
        {
            Logger.Log($"nutaku: initialization_status - {(status ? "succeeded" : "failed")} at {Time.time:F2}s");
#if !UNITY_EDITOR
            string statusLabel = status ? "succeeded" : "failed";
            GameAnalytics.NewDesignEvent($"nutaku:initialization_status:{statusLabel}", Time.time);
            GameAnalytics.NewDesignEvent($"nutaku:initialization_flag:{status}", 1);
#endif
        }

        public void SendNutakuConnectionStatus(bool status)
        {
            Logger.Log($"nutaku: connection status is {(status ? "connected" : "failed")} at {Time.time:F2}s");
#if !UNITY_EDITOR
            string statusLabel = status ? "connected" : "failed";
            GameAnalytics.NewDesignEvent($"nutaku:connection_status:{statusLabel}", Time.time);
            GameAnalytics.NewDesignEvent($"nutaku:connection_flag:{status}", 1);
#endif
        }

        public void SendNutakuTimeFetchedStatus(bool status)
        {
            Logger.Log($"nutaku: time fetch status is {(status ? "fetched" : "failed")} at {Time.time:F2}s");
#if !UNITY_EDITOR
            string statusLabel = status ? "fetched" : "failed";
            GameAnalytics.NewDesignEvent($"nutaku:time_fetch_status:{statusLabel}", Time.time);
            GameAnalytics.NewDesignEvent($"nutaku:time_fetch_flag:{status}", 1);
#endif
        }

        public void SendNutakuVersionCheckStatus(bool status)
        {
            Logger.Log($"nutaku: version check status is {(status ? "succeeded" : "failed")} at {Time.time:F2}s");
#if !UNITY_EDITOR
            string statusLabel = status ? "succeeded" : "failed";
            GameAnalytics.NewDesignEvent($"nutaku:version_check_status:{statusLabel}", Time.time);
            GameAnalytics.NewDesignEvent($"nutaku:version_check_flag:{status}", 1);
#endif
        }

        public void SendNutakuVersionAndPlatformId(string version, string platformId)
        {
            Logger.Log($"nutaku: session start - platform: {platformId}, version: {version}");
#if !UNITY_EDITOR
            string sanitizedPlatform = platformId.ToLowerInvariant();
            string sanitizedVersion = version;

            GameAnalytics.NewDesignEvent($"nutaku:session_start:{sanitizedPlatform}:{sanitizedVersion}");
            GameAnalytics.NewDesignEvent($"nutaku:session_start_flag:{platformId.ToLowerInvariant()}", 1);
#endif
        }

        #endregion
    
        private string Sanitize(string input)
        {
            // Только допустимые символы: a-zA-Z0-9-_.,:()!?
            var safe = System.Text.RegularExpressions.Regex.Replace(input, @"[^a-zA-Z0-9\-_\.\:\(\)\!\?]", "_");
            return safe.Length <= 64 ? safe : safe.Substring(0, 64);
        }
    }
}