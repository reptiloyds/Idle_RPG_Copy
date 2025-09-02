using System;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Quests.Model.SpinRoulette
{
    [Serializable]
    public class QSpinRouletteData
    {
        public RouletteType Id; //TODO: rename to Type 
        public int Amount;
        
        [Preserve]
        public QSpinRouletteData()
        {
        }
    }
}