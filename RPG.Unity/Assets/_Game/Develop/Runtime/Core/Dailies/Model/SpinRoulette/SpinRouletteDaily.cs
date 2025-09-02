using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Save;
using PleasantlyGames.RPG.Runtime.Core.Dailies.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Contract;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.Model;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Type;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.Model.SpinRoulette
{
    public class SpinRouletteDaily : Daily
    {
        private readonly DSpinRouletteData _configData;
        private readonly RouletteFacade _facade;
        private readonly List<IRoulette> _rouletteList = new();
        
        public SpinRouletteDaily(DailyRow config, DailyData data, ResourceService resourceService, ITranslator translator, RouletteFacade facade)
            : base(config, data, resourceService, translator)
        {
            _facade = facade;
            _configData = JsonConvertLog.DeserializeObject<DSpinRouletteData>(config.DataJSON);
        }

        public override void Initialize()
        {
            base.Initialize();
            if (_configData.Type == RouletteType.None)
            {
                foreach (var kvp in _facade.RouletteDictionary) 
                    _rouletteList.Add(kvp.Value);
            }
            else
            {
                _rouletteList.Add(_facade.RouletteDictionary[_configData.Type]);
                Description += Translator.Translate(_configData.Type.ToString());
            }

            if (IsComplete) return;
            foreach (var roulette in _rouletteList) 
                roulette.OnSpin += OnSpin;
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (var roulette in _rouletteList) 
                roulette.OnSpin -= OnSpin;
        }

        private void OnSpin() => 
            Progress++;

        protected override int GetTargetValue() => 
            _configData.Amount;

        protected override void OnComplete()
        {
            foreach (var roulette in _rouletteList) 
                roulette.OnSpin -= OnSpin;
            base.OnComplete();
        }
    }
}