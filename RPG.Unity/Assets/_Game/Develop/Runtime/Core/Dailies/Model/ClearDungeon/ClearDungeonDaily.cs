using System;
using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Save;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Sheet;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Contract;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.Model.ClearDungeon
{
    public class ClearDungeonDaily : Daily
    {
        private readonly DungeonModeFacade _facade;
        private readonly DClearDungeonData _configData;
        private ILeveledDungeon _dungeon;

        public ClearDungeonDaily(DailyRow config, DailyData data, ResourceService resourceService, ITranslator translator, DungeonModeFacade facade)
            : base(config, data, resourceService, translator)
        {
            _facade = facade;
            _configData = JsonConvert.DeserializeObject<DClearDungeonData>(config.DataJSON);
        }

        public override void Initialize()
        {
            base.Initialize();
            _dungeon = _facade.GetLeveledDungeon(_configData.Type);
            if (!IsComplete) 
                _dungeon.OnWin += OnDungeonWin;
            Description += $" {Translator.Translate(_configData.Type.ToString())}";
        }

        public override void Dispose()
        {
            _dungeon.OnWin -= OnDungeonWin;
            base.Dispose();
        }

        private void OnDungeonWin(IGameMode gameMode)
        {
            Progress++;
        }

        protected override int GetTargetValue() => 
            _configData.Amount;

        protected override void OnComplete()
        {
            _dungeon.OnWin -= OnDungeonWin;
            base.OnComplete();
        }
    }
}