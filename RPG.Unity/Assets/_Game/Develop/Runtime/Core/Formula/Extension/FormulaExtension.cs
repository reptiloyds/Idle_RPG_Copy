using System;
using System.Collections.Generic;
using System.Globalization;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Formula.Extension
{
    public static class FormulaExtension
    {
        public static void DeserializeFormula(this FormulaType formulaType, string formulaJSON)
        {
            switch (formulaType)
            {
                case FormulaType.Pow:
                    JsonConvertLog.DeserializeObject<PowFormulaData>(formulaJSON);
                    break;
                case FormulaType.LevelAddition:
                    JsonConvertLog.DeserializeObject<LevelAdditionData>(formulaJSON);
                    break;
                case FormulaType.Pow2:
                    JsonConvertLog.DeserializeObject<Pow2FormulaData>(formulaJSON);
                    break;
                case FormulaType.Pow2PayWall:
                    JsonConvertLog.DeserializeObject<Pow2PayWallFormulaData>(formulaJSON);
                    break;
                case FormulaType.ManualBigDoubles:
                    JsonConvertLog.DeserializeObject<List<ManualData>>(formulaJSON);
                    break;
                case FormulaType.ManualFloats:
                    JsonConvertLog.DeserializeObject<List<float>>(formulaJSON);
                    break;
            }
        }
        
        public static BaseValueFormula CreateFormula(this FormulaType formulaType, string dataJSON)
        {
            switch (formulaType)
            {
                case FormulaType.None:
                    float startValue = 0;
                    if (!string.IsNullOrWhiteSpace(dataJSON)) 
                        startValue = float.Parse(dataJSON, CultureInfo.InvariantCulture); 
                    return new BaseValueFormula(startValue);
                case FormulaType.Pow:
                    return new PowFormula(dataJSON);
                case FormulaType.Pow2:
                    return new Pow2Formula(dataJSON);
                case FormulaType.Pow2PayWall:
                    return new Pow2PayWallFormula(dataJSON);
                case FormulaType.LevelAddition:
                    return new LevelAdditionFormula(dataJSON);
                case FormulaType.ManualBigDoubles:
                    return new ManualBigDoublesFormula(dataJSON);
                case FormulaType.ManualFloats:
                    return new ManualFloatsFormula(dataJSON);
                default:
                    Debug.LogError($"{typeof(FormulaType)} {formulaType} is not defined");
                    return null;
            }
        }
    }
}