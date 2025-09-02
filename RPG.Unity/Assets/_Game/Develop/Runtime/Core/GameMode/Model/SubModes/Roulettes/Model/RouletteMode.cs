using System;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Model;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Contract;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Type;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Roulettes.Model
{
    public abstract class RouletteMode : SubMode, IRoulette
    {
        public abstract RouletteType Type { get; }
        public event Action OnSpin;
        
        public void Spin()
        {
            if (IsEnterResourceEnough)
                SpendEnterResource();
            else if (BonusEnterAmount > 0)
                SpendBonusEnter();
            else
                return;
            OnSpin?.Invoke();
        }
    }
}