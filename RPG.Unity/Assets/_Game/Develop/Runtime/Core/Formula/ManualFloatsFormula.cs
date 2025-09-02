using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;

namespace PleasantlyGames.RPG.Runtime.Core.Formula
{
    public class ManualFloatsFormula : BaseValueFormula
    {
        private readonly List<float> _data;

        public ManualFloatsFormula(string dataJSON) => 
            _data = JsonConvertLog.DeserializeObject<List<float>>(dataJSON);

        public override BigDouble.Runtime.BigDouble CalculateBigDouble(int level)
        {
            BigDouble.Runtime.BigDouble result;
            if (level <= 0) result = 0;
            else if (level >= _data.Count) result = _data[^1];
            else result = _data[level - 1];

            return StartValue + result;
        }
    }
}