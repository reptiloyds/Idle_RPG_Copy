using System;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Formula.Sheets;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.Type;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.AreaDamage;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.Bomber;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.Explosion;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.Projectile;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.ProjectileDrop;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.ShootingObject;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.StatModifier;
using PleasantlyGames.RPG.Runtime.Core.Skill.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Skill.Type;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model
{
    public class SkillFactory
    {
        [Inject] private BalanceContainer _balance;
        [Inject] private IObjectResolver _objectResolver;
        
        private ExtendedManualFormulaSheet<SkillSheet, string, SkillValueType> _customValueSheet;

        [Preserve]
        public SkillFactory() { }

        public void Initialize() => 
            _customValueSheet = _balance.Get<ExtendedManualFormulaSheet<SkillSheet, string, SkillValueType>>();

        public Skill Create(SkillRow config, Func<int> levelSource = null)
        {
            var skillModel = new Skill(config, levelSource);
            _objectResolver.Inject(skillModel);
            AppendEffect(skillModel, config);

            skillModel.Initialize();
            return skillModel;
        }

        private void AppendEffect(Skill model, SkillRow config)
        {
            BaseSkillEffect effect;
            switch (config.EffectType)
            {
                case SkillEffectType.StatModifier:
                    effect = new StatModifierEffect(config, model.NameId, _customValueSheet);
                    break;
                case SkillEffectType.Projectile:
                    effect = new ProjectileEffect(config, model.NameId, _customValueSheet);
                    break;
                case SkillEffectType.AreaDamage:
                    effect = new AreaDamageEffect(config, model.NameId, _customValueSheet);
                    break;
                case SkillEffectType.ProjectileDrop:
                    effect = new ProjectileDropEffect(config, model.NameId, _customValueSheet);
                    break;
                case SkillEffectType.Explosion:
                    effect = new ExplosionEffect(config, model.NameId, _customValueSheet);
                    break;
                case SkillEffectType.ShootingObject:
                    effect = new ShootingObjectEffect(config, model.NameId, _customValueSheet);
                    break;
                case SkillEffectType.Bomber:
                    effect = new BomberEffect(config, model.NameId, _customValueSheet);
                    break;
                default:
                    Debug.LogError($"{typeof(SkillEffectType)} {config.EffectType} is not defined");
                    return;
            } 
            _objectResolver.Inject(effect);
            model.AddEffect(effect);
        }
    }
}