#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Cathei.BakingSheet.Unity;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
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
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Skill.Sheet;
using PleasantlyGames.RPG.Runtime.Core.StagesRewardsFeature.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Units.Sheet;
using PleasantlyGames.RPG.Runtime.VIP.Sheets;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Balance
{
    [CreateAssetMenu(fileName = "EditorBalanceParser", menuName = "SO/Parser/EditorBalance")]
    public class EditorBalanceParser : BaseEditorBalanceParser
    {
        protected override async void Parse(string sheetId)
        {
            base.Parse(sheetId);
            
            var startTime = DateTime.UtcNow;
            Debug.Log("Start parse");
            var logger = new UnityLogger();

            var containers = new List<CustomSheetContainer>
            {
                new UnitContainer(logger),
                new MainModeContainer(logger),
                new BossRushContainer(logger),
                new SoftRushContainer(logger),
                new ResourceContainer(logger),
                new ItemContainer(logger),
                new ContentContainer(logger),
                new SkillContainer(logger),
                new GlobalStatContainer(logger),
                new CharacterContainer(logger),
                new QuestContainer(logger),
                new TutorialContainer(logger),
                new LootboxContainer(logger),
                new ProductsContainer(logger),
                new CollectionContainer(logger),
                new RouletteContainer(logger),
                new PeriodicRewardContainer(logger),
                new BlessingSheetsContainer(logger),
                new DailiesContainer(logger),
                new PhoneContainer(logger),
                new PiggyBankContainer(logger),
                new PiggyBankBonusesContainer(logger),
                new StageRewardsContainer(logger),
                new VipContainer(logger),
            };

            await Parse(containers, sheetId);
            
            var duration = DateTime.UtcNow - startTime;
            Debug.Log($"Finished import in {duration.TotalSeconds} seconds");
        }

        [Button]
        private void TestSerialization()
        {
        }
        
        [Serializable]
        private class TestResourcePrice
        {
            public ResourceType Type;
            public double Amount;
        }
    }
}

#endif