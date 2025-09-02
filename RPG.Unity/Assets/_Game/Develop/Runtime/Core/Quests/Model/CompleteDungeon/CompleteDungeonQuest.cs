using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Contract;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model;
using PleasantlyGames.RPG.Runtime.Core.Quests.Sheet;

namespace PleasantlyGames.RPG.Runtime.Core.Quests.Model.CompleteDungeon
{
    public class CompleteDungeonQuest : Quest
    {
        private readonly DungeonModeFacade _modeFacade;
        private readonly QCompleteDungeonData _data;
        private ILeveledDungeon _dungeon;

        public CompleteDungeonQuest(DungeonModeFacade dungeonModeFacade, QuestRow config, int progress) : base(config, progress)
        {
            _modeFacade = dungeonModeFacade;
            _data = JsonConvert.DeserializeObject<QCompleteDungeonData>(config.DataJSON);
        }
        
        public override void Initialize()
        {
            base.Initialize();
            
            _dungeon = _modeFacade.GetLeveledDungeon(_data.Type);
            Description += $": {_dungeon.GetFormattedModeName()} {_data.Level}";
            _dungeon.OnWin += OnModeWin;
            if(_dungeon.AvailableLevel > _data.Level)
                Complete();
        }

        public override void Dispose()
        {
            _dungeon.OnWin -= OnModeWin;
            base.Dispose();
        }

        private void OnModeWin(IGameMode obj)
        {
            if(_dungeon.AvailableLevel > _data.Level)
                Complete();
        }

        public override string GetDescription() => 
            Description;

        public override (float progress, string progressText) GetProgress() => 
            ((float)_dungeon.AvailableLevel / _data.Level, string.Empty);
    }
}