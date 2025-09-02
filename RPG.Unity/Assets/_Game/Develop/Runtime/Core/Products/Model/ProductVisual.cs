using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Products.Type;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using VContainer;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Model
{
    public class ProductVisual
    {
        [Inject] private BalanceContainer _balance;
        [Inject] private ISpriteProvider _spriteProvider;
        [Inject] private ITranslator _translator;

        private readonly ProductRow _config;
        private readonly ProductRewards _rewards;

        public VisualData VisualData { get; private set; }
        public HorizontalVisualData HorizontalData { get; private set; }
        public VerticalVisualData VerticalData { get; private set; }
        public MockVisualData MockData { get; private set; }

        public ProductVisual(ProductRow config, ProductRewards rewards)
        {
            _config = config;
            _rewards = rewards;
        }

        public void Initialize() =>
            CreateData();

        private void CreateData()
        {
            switch (_config.Visual)
            {
                case ProductVisualType.HorizontalCard:
                    CreateHorizontalData();
                    break;
                case ProductVisualType.VerticalCard:
                    CreateVerticalData();
                    break;
                case ProductVisualType.None:
                    CreateMockData();
                    break;
            }
        }

        private void CreateMockData()
        {
            MockData = new MockVisualData()
            {
                ContentSprites = new List<Sprite>(),
            };
            VisualData = MockData;
        }

        private void CreateHorizontalData()
        {
            var horizontalSheet = _balance.Get<HorizontalProductVisualSheet>();
            if (!horizontalSheet.Contains(_config.Id))
            {
                Logger.LogError($"Can`t find visual config for {_config.Id}");
                return;
            }

            var visualConfig = horizontalSheet[_config.Id];
            var sprites = new List<Sprite>();
            FillSprites(sprites, visualConfig.ContentImageType, visualConfig.ContentManualImages);

            string labelText = null;
            if (_rewards.Character != null) 
                labelText = _rewards.Character.CharacterModel.FormattedName;
            HorizontalData = new HorizontalVisualData()
            {
                BackColor = visualConfig.BackgroundColor,
                BackSprite = _spriteProvider.GetSprite(visualConfig.BackSprite),
                ContentColor = visualConfig.ContentBackgroundColor,
                ContentBackground = visualConfig.ContentBackground,
                ContentSprites = sprites,
                ItemPresent = visualConfig.ItemPresent,
                ContentLabelText = labelText,
                HideWhenOver = visualConfig.HideWhenOver,
                BadgeEnabled = visualConfig.BadgeEnabled,
                BadgeId = visualConfig.BadgeStyle,
                BadgeText = _translator.Translate(visualConfig.BadgeText),
                BadgeValue = visualConfig.BadgeValue,
            };
            VisualData = HorizontalData;
        }

        private void CreateVerticalData()
        {
            var verticalSheet = _balance.Get<VerticalProductVisualSheet>();
            if (!verticalSheet.Contains(_config.Id))
            {
                Logger.LogError($"Can`t find visual config for {_config.Id}");
                return;
            }

            var visualConfig = verticalSheet[_config.Id];
            var sprites = new List<Sprite>();
            FillSprites(sprites, visualConfig.ContentImageType, visualConfig.ContentManualImages);

            VerticalData = new VerticalVisualData()
            {
                BackColor = visualConfig.BackgroundColor,
                ContentSprites = sprites,
                HideWhenOver = visualConfig.HideWhenOver,
                BadgeEnabled = visualConfig.BadgeEnabled,
                BadgeId = visualConfig.BadgeStyle,
                BadgeText = _translator.Translate(visualConfig.BadgeText),
                BadgeValue = visualConfig.BadgeValue,
            };
            VisualData = VerticalData;
        }

        private void FillSprites(List<Sprite> sprites, ProductContentImageType type, List<string> manualImages)
        {
            switch (type)
            {
                case ProductContentImageType.None:
                    break;
                case ProductContentImageType.FirstReward:
                    if (_rewards.List.Count > 0)
                        sprites.Add(_rewards.List[0].Sprite);
                    break;
                case ProductContentImageType.MergeReward:
                    var mergeReward = _rewards.GetMergeReward();
                    if (mergeReward != null)
                        sprites.Add(mergeReward.Sprite);
                    break;
                case ProductContentImageType.AllRewards:
                    foreach (var reward in _rewards.List)
                        sprites.Add(reward.Sprite);
                    break;
                case ProductContentImageType.Manual:
                    var manualSprites = _spriteProvider.GetSprites(Asset.MainAtlas, manualImages);
                    sprites.AddRange(manualSprites);
                    break;
            }
        }
    }
}