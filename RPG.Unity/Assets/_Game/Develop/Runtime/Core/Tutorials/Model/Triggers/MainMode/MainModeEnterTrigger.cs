using Newtonsoft.Json;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers.MainMode
{
    internal class MainModeEnterTrigger : TutorialTrigger
    {
        private readonly GameMode.Model.Main.MainMode _mainMode;
        private readonly MainModeEnterData _data;
        
        public MainModeEnterTrigger(GameMode.Model.Main.MainMode mainMode, string dataJSON)
        {
            _mainMode = mainMode;
            _data = JsonConvert.DeserializeObject<MainModeEnterData>(dataJSON);
        }

        public override void Initialize()
        {
            _mainMode.OnLevelEntered += CheckCondition;
            CheckCondition();
        }

        public override void Dispose() => 
            _mainMode.OnLevelEntered -= CheckCondition;

        private void CheckCondition()
        {
            if (_mainMode.Id > _data.Id || _mainMode.Id == _data.Id && _mainMode.Level >= _data.Level) 
                Execute();
        }
    }
}