using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Save;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Roulettes.Model.SlotRush.Definition;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Roulettes.Model.SlotRush.Save;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Roulettes.Model.SlotRush.View;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.SlotMachine;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Roulettes.Model.SlotRush.Model
{
    public class SlotRushRoulette : RouletteMode
    {
        [Inject] private SlotRushDataProvider _dataProvider;
        [Inject] private SlotRushConfiguration _configuration;
        [Inject] private ResourceService _resourceService;
        [Inject] private ISpriteProvider _spriteProvider;
        [Inject] private IWindowService _windowService;
        
        private SlotDataContainer _data;
        private RouletteSheet.Row _config;
        private Sprite _rewardSprite;
        private const string REWARD_SPRITE_NAME = "SlotReward";
        
        private readonly List<SlotData> _slotDataList = new();
        private readonly Dictionary<int, RouletteSheet.Elem> _slotDictionary = new();

        protected override SubModeSetup Setup => _configuration.SubModeSetup;
        protected override SubModeDataContainer SubModeData => _data.SubModeData;
        protected override string SubModeId => "SlotRush";
        
        public List<SlotData> SlotDataList => _slotDataList;
        public override Sprite RewardImage => _rewardSprite;
        
        public override RouletteType Type => RouletteType.SlotRush;

        [Preserve]
        public SlotRushRoulette()
        {
        }

        public override void Initialize()
        {
            _data = _dataProvider.GetData();
            _config = Balance.Get<RouletteSheet>()[Type];
            _rewardSprite = _spriteProvider.GetSprite(Asset.MainAtlas, REWARD_SPRITE_NAME);
            
            for (int i = 0; i < _config.Count; i++)
            {
                var elem = _config[i];
                var resource = ResourceService.GetResource(elem.ResourceType);
                _slotDataList.Add(new SlotData()
                {
                    Amount = elem.Amount,
                    Sprite = resource.Sprite,
                    Weight = elem.Weight,
                    Index = i,
                });
                _slotDictionary.Add(i, elem);
            }
            
            Name = Translator.Translate(Type.ToString());
            
            base.Initialize();
        }

        public override async UniTask Select() => 
            await _windowService.OpenAsync<SlotRushWindow>();

        public void ApplyReward(int index, RectTransform source)
        {
            var pieceConfig = _slotDictionary[index];
            _resourceService.AddResource(pieceConfig.ResourceType, pieceConfig.Amount, ResourceFXRequest.Create(spawnPosition:source.position));
        }
    }
}