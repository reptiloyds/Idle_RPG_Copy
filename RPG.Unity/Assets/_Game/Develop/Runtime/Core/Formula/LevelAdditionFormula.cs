using System;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Formula
{
    [Serializable]
    public class LevelAdditionData
    {
        public float Value;
        public float ValuePerLevel;
        
        [Preserve]
        public LevelAdditionData() { }
    }
    
    public class LevelAdditionFormula : BaseValueFormula
    {
        private readonly LevelAdditionData _data;
        
        public LevelAdditionFormula(string dataJSON)
        {
            _data = JsonConvertLog.DeserializeObject<LevelAdditionData>(dataJSON);
            StartValue = _data.Value;
        }

        public override BigDouble.Runtime.BigDouble CalculateBigDouble(int level)
        {
            if (level <= 1)return base.CalculateBigDouble(level);
            return StartValue + (level - 1) * _data.ValuePerLevel;
        }
    }
}