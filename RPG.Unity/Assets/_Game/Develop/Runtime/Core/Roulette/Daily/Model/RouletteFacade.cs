using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Roulettes.Model;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Contract;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Type;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Roulette.Daily.Model
{
    public class RouletteFacade
    {
        private readonly IEnumerable<RouletteMode> _rouletteModes;

        private readonly Dictionary<RouletteType, IRoulette> _rouletteDictionary = new();
        
        public IEnumerable<RouletteMode> RouletteModes => _rouletteModes;
        public IReadOnlyDictionary<RouletteType, IRoulette> RouletteDictionary => _rouletteDictionary;

        [Inject][Preserve]
        public RouletteFacade(IEnumerable<RouletteMode> rouletteModes, IEnumerable<IRoulette> roulette)
        {
            _rouletteModes = rouletteModes;
            foreach (var item in roulette) 
                _rouletteDictionary[item.Type] = item;
        }
        
        public void Initialize()
        {
            foreach (var rouletteMode in _rouletteModes) 
                rouletteMode.Initialize();
        }
    }
}