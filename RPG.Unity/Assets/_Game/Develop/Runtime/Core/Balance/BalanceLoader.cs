using System;
using System.Collections.Generic;
using Cathei.BakingSheet.Unity;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Balance.Contract;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Characters.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Collections.Sheets;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Sheet;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main.Sheet;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.BossRush.Sheet;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.SoftRush.Sheet;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Items.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Lootboxes.Sheet;
using PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Phone.Chats.Sheet;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Sheets;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.PiggyBank.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Products.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Quests.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Resource.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Skill.Sheet;
using PleasantlyGames.RPG.Runtime.Core.StagesRewardsFeature.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Units.Sheet;
using PleasantlyGames.RPG.Runtime.VIP.Sheets;
using UnityEngine;
using UnityEngine.Networking;
using VContainer;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Core.Balance
{
    public class BalanceLoader : IBalanceLoader
    {
        private readonly IObjectResolver _objectResolver;
        
        private DateTime _startTime;
        private string _jsonBalance;

        private readonly List<CustomSheetContainer> _containers = new(32);

        [Preserve]
        [Inject]
        public BalanceLoader(IObjectResolver resolver)
        {
            _objectResolver = resolver;
        }
        
        public async UniTask Load(string jsonBalance)
        {
            _startTime = DateTime.UtcNow;
            var logger = new UnityLogger();
            _jsonBalance = jsonBalance;
            
            _containers.Add(new UnitContainer(logger));
            _containers.Add(new MainModeContainer(logger));
            _containers.Add(new BossRushContainer(logger));
            _containers.Add(new SoftRushContainer(logger));
            _containers.Add(new ResourceContainer(logger));
            _containers.Add(new ItemContainer(logger));
            _containers.Add(new ContentContainer(logger));
            _containers.Add(new SkillContainer(logger));
            _containers.Add(new GlobalStatContainer(logger));
            _containers.Add(new CharacterContainer(logger));
            _containers.Add(new QuestContainer(logger));
            _containers.Add(new TutorialContainer(logger));
            _containers.Add(new LootboxContainer(logger));
            _containers.Add(new ProductsContainer(logger));
            _containers.Add(new CollectionContainer(logger));
            _containers.Add(new RouletteContainer(logger));
            _containers.Add(new PeriodicRewardContainer(logger));
            _containers.Add(new BlessingSheetsContainer(logger));
            _containers.Add(new DailiesContainer(logger));
            _containers.Add(new PhoneContainer(logger));
            _containers.Add(new PiggyBankContainer(logger));
            _containers.Add(new PiggyBankBonusesContainer(logger));
            _containers.Add(new StageRewardsContainer(logger));
            _containers.Add(new VipContainer(logger));

            await ReadFile();
            
            var duration = DateTime.UtcNow - _startTime;
            Logger.Log($"Finished load balance in {duration.TotalSeconds} seconds");
        }


        private async UniTask ReadFile()
        {
            foreach (var container in _containers) 
                _objectResolver.Inject(container);
            
            var jsonConverter = new CustomJsonSheetConverter("", BalanceConst.FileName);
            if (!string.IsNullOrEmpty(_jsonBalance))
                jsonConverter.ReadFromJson(_jsonBalance);
            else
                await LoadLocalFile(jsonConverter);
            
            foreach (var container in _containers) 
                await container.Bake(jsonConverter);
        }

        private async UniTask LoadLocalFile(CustomJsonSheetConverter converter)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                UnityWebRequest webRequest = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + $"{BalanceConst.FileName}.json");
                await webRequest.SendWebRequest().ToUniTask();
                if (webRequest.result == UnityWebRequest.Result.Success)
                    converter.ReadFromJson(webRequest.downloadHandler.text);
                else
                    Debug.LogError($"WebRequest by path {webRequest.url} is fall");
            }
            else
                await converter.ReadFromStreamingAssets();
        }
    }
}