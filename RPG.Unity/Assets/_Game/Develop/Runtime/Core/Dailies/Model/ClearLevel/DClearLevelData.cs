using System;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.Model.ClearLevel
{
    [Serializable]
    public class DClearLevelData
    {
        public int Amount;
        
        [Preserve]
        public DClearLevelData() { }
    }
}