using System;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Type;

namespace PleasantlyGames.RPG.Runtime.Core.Roulette.Contract
{
    public interface IRoulette
    {
        RouletteType Type { get; }
        event Action OnSpin;

        void Spin();
    }
}