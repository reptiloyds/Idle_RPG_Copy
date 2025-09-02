using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Formula
{   
    public class Pow2PayWallFormulaData
    {
        public float Value;
        public float K;
        public int EveryLevel;
        public int Addition;
        
        [Preserve]
        public Pow2PayWallFormulaData()
        {
        }
    }
    
    public class Pow2PayWallFormula : BaseValueFormula
    {
        private readonly Pow2PayWallFormulaData _data;
        
        public Pow2PayWallFormula(string dataJSON)
        {
            _data = JsonConvertLog.DeserializeObject<Pow2PayWallFormulaData>(dataJSON);
            if(_data.EveryLevel == 0) _data.EveryLevel = 1;
            StartValue = _data.Value;
        }

        // public override double Calculate(int level)
        // {
        //     if (level <= 1) return base.Calculate(level);
        //     var pow = level + _data.Addition * (level / _data.EveryLevel);
        //     return StartValue * Mathf.Pow(_data.ValueK, pow);
        // }

        public override BigDouble.Runtime.BigDouble CalculateBigDouble(int level)
        {
            if (level <= 1) return base.CalculateBigDouble(level);
            var pow = level + _data.Addition * (level / _data.EveryLevel);
            return StartValue * BigDouble.Runtime.BigDouble.Pow(_data.K, pow);
        }
    }
}