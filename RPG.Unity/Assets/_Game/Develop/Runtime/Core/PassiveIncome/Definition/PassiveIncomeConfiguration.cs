using System;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace PleasantlyGames.RPG.Runtime.Core.PassiveIncome.Definition
{
    [Serializable]
    public class PassiveIncomeConfiguration
    {
        [MinValue(0)] public float OfflineSoftK = 1;
        [MinValue(0)] public float OnlineSoftK = 1;
        [MinValue(0)] public float BonusIncomeK = 2;
        [MinValue(1)] public int IncomeHoursLimit = 12;
        [MinValue(1)] public int TickDurationInMinutes = 1;
        [MinValue(0)] public int CooldownInMinutes = 5;
    }
}