using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

namespace PleasantlyGames.RPG.Runtime.Core.Formula
{
    [Serializable]
    public class ManualData
    {
        public double M;
        public long E;
        
        [Preserve]
        public ManualData()
        {
        }

        public ManualData(double m, long e)
        {
            M = m;
            E = e;
        }

        public BigDouble.Runtime.BigDouble ToBigDouble() => 
            new(M, E);

        public static BigDouble.Runtime.BigDouble ToBigDouble(ManualData data) =>
            new(data.M, data.E);
    }
    
    public class ManualBigDoublesFormula : BaseValueFormula
    {
        private readonly List<ManualData> _data;

        public ManualBigDoublesFormula(string dataJSON) => 
            _data = JsonConvertLog.DeserializeObject<List<ManualData>>(dataJSON);

        public ManualBigDoublesFormula(List<ManualData> data) => 
            _data = data;

        public override BigDouble.Runtime.BigDouble CalculateBigDouble(int level)
        {
            BigDouble.Runtime.BigDouble result;
            if (level <= 0) result = 0;
            else if (level >= _data.Count) result = _data[^1].ToBigDouble();
            else result = _data[level - 1].ToBigDouble();

            return StartValue + result;
        }
    }
}