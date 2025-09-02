using System;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Type;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.Model.SpinRoulette
{
    [Serializable]
    public class DSpinRouletteData
    {
        public RouletteType Type; 
        public int Amount;

        [Preserve]
        public DSpinRouletteData() { }
    }
}