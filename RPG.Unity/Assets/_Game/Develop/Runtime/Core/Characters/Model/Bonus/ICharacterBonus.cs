using System;
using PleasantlyGames.RPG.Runtime.Core.Characters.Sheet.Data;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.Model.Bonus
{
    public interface ICharacterBonus
    {
        BonusConditionType ConditionType { get; }
        bool IsUnlocked { get; }
        int Level { get; }
        Sprite Sprite { get; }
        event Action<ICharacterBonus> OnLevelUp;
        
        void Initialize(Character character);
        bool IsAlwaysApply();
        int GetMetConditions();
        string GetEnhanceCondition();
        string GetUnlockCondition();
        string GetStatName();
        string GetStatValue(int level);
        void OnEquip();
        void OnTakeOff();
    }
}