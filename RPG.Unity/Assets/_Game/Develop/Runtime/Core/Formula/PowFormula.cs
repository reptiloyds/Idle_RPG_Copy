using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Formula
{
    public class PowFormulaData
    {
        public float Value;
        public float K;
        
        [Preserve]
        public PowFormulaData()
        {
        }
    }
    
    public class PowFormula : BaseValueFormula
    {
        private readonly PowFormulaData _data;
        
        public PowFormula(string dataJSON)
        {
            _data = JsonConvertLog.DeserializeObject<PowFormulaData>(dataJSON);
            StartValue = _data.Value;
        }

        public override BigDouble.Runtime.BigDouble CalculateBigDouble(int level)
        {
            if (level <= 1) return base.CalculateBigDouble(level);
            return level * BigDouble.Runtime.BigDouble.Pow(StartValue, _data.K);
        }
    }
}