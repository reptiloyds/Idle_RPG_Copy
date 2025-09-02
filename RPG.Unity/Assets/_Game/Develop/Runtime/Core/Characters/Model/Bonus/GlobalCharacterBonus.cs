using PleasantlyGames.RPG.Runtime.Core.Characters.Model.Bonus.Conditions;
using PleasantlyGames.RPG.Runtime.Core.Characters.Sheet;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Model;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Type;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.Model.Bonus
{
    public class GlobalCharacterBonus : CharacterBonus<GlobalStatType>
    {
        [Inject] private GlobalStatProvider _statProvider;
        
        public GlobalCharacterBonus(CharacterRow.Elem config, IBonusCondition condition, Sprite sprite) : base(config, condition, sprite)
        {
        }

        protected override void FillTargetStats() => 
            Stats.Add(_statProvider.GetStat(EffectType));
    }
}