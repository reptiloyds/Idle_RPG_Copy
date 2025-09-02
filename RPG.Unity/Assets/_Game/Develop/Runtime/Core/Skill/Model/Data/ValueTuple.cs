using System;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Data
{
    [Serializable]
    public class ValueTuple
    {
        public string Key;
        public FormulaType FormulaType;
        public string FormulaData;

        [Preserve]
        public ValueTuple()
        {
            
        }
    }
}