using System;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Dailies.Model.DefeatEnemy
{
    [Serializable]
    public class DDefeatEnemyData
    {
        public int Amount;

        [Preserve]
        public DDefeatEnemyData() { }
    }
}