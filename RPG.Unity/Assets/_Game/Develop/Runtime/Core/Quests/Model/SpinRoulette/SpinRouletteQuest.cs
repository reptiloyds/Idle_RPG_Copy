using System.Collections.Generic;
using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.Core.Quests.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Contract;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.Model;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Type;

namespace PleasantlyGames.RPG.Runtime.Core.Quests.Model.SpinRoulette
{
    public class SpinRouletteQuest : Quest
    {
        private readonly QSpinRouletteData _data;
        private readonly RouletteFacade _facade;
        private readonly List<IRoulette> _rouletteList = new();

        public SpinRouletteQuest(QuestRow config, int progress, RouletteFacade facade) : base(config, progress)
        {
            _facade = facade;
            _data = JsonConvert.DeserializeObject<QSpinRouletteData>(config.DataJSON);
        }

        public override void Initialize()
        {
            base.Initialize();

            if (_data.Id == RouletteType.None)
            {
                foreach (var kvp in _facade.RouletteDictionary) 
                    _rouletteList.Add(kvp.Value);
            }
            else
            {
                _rouletteList.Add(_facade.RouletteDictionary[_data.Id]);
                Description += $" {Translator.Translate(_data.Id.ToString())}";
            }
            foreach (var roulette in _rouletteList) 
                roulette.OnSpin += OnSpin;
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (var roulette in _rouletteList) 
                roulette.OnSpin -= OnSpin;
        }

        private void OnSpin()
        {
            Progress++;
            if (Progress >= _data.Amount && !IsComplete) 
                Complete();
        }

        public override string GetDescription() => 
            Description;

        public override (float progress, string progressText) GetProgress() => 
            ((float)Progress / _data.Amount, $"{Progress.ToString()}/{_data.Amount.ToString()}");
    }
}