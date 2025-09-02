using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Formula
{
    public class Pow2FormulaData
    {
        public float Value;
        public float K;
        
        [Preserve]
        public Pow2FormulaData()
        {
        }
    }
    
    public class Pow2Formula : BaseValueFormula
    {
        private readonly Pow2FormulaData _data;
        
        public Pow2Formula(string dataJSON)
        {
            _data = JsonConvertLog.DeserializeObject<Pow2FormulaData>(dataJSON);
            StartValue = _data.Value;
        }

        public override BigDouble.Runtime.BigDouble CalculateBigDouble(int level)
        {
            if (level <= 1) return base.CalculateBigDouble(level);
            return StartValue * BigDouble.Runtime.BigDouble.Pow(_data.K, level);
        }
    }
}