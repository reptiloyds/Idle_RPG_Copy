using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model.Bonus.Conditions;
using PleasantlyGames.RPG.Runtime.Core.Characters.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.Model.Bonus
{
    public class UnitCharacterBonus : CharacterBonus<UnitStatType>
    {
        [Inject] private BranchService _branchService;
        [Inject] private UnitStatService _statService;

        public UnitCharacterBonus(CharacterRow.Elem config, IBonusCondition condition, Sprite sprite) : base(config, condition, sprite)
        {
        }

        protected override void FillTargetStats()
        {
            var stats = _statService.GetPlayerStats();
            foreach (var stat in stats)
            {
                if (stat.Type != EffectType) continue;
                Stats.Add(stat);
                break;
            }
        }
    }
}