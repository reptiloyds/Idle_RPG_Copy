using System;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.Model.CompleteDailies
{
    [Serializable]
    public class DCompleteDailiesData
    {
        public int Amount;
        
        [Preserve]
        public DCompleteDailiesData() { }
    }
}