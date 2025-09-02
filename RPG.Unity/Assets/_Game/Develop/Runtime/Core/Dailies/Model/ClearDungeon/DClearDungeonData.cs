using System;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Type;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.Model.ClearDungeon
{
    [Serializable]
    public class DClearDungeonData
    {
        public GameModeType Type;
        public int Amount;

        [Preserve]
        public DClearDungeonData() { }
    }
}