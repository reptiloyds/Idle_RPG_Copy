using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.AreaDamage;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.Bomber;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.Explosion;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.Projectile;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.ProjectileDrop;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.ShootingObject;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Effect.StatModifier;
using PleasantlyGames.RPG.Runtime.Core.Skill.Type;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Extension
{
    public static class SkillExtension
    {
        public static void TryDeserialize(this SkillEffectType type, string json)
        {
            switch (type)
            {
                case SkillEffectType.StatModifier:
                    JsonConvertLog.DeserializeObject<StatModifierData>(json);
                    break;
                case SkillEffectType.Projectile:
                    JsonConvertLog.DeserializeObject<ProjectileData>(json);
                    break;
                case SkillEffectType.AreaDamage:
                    JsonConvertLog.DeserializeObject<AreaDamageData>(json);
                    break;
                case SkillEffectType.ProjectileDrop:
                    JsonConvertLog.DeserializeObject<ProjectileDropData>(json);
                    break;
                case SkillEffectType.Explosion:
                    JsonConvertLog.DeserializeObject<ExplosionData>(json);
                    break;
                case SkillEffectType.ShootingObject:
                    JsonConvertLog.DeserializeObject<ShootingObjectData>(json);
                    break;
                case SkillEffectType.Bomber:
                    JsonConvertLog.DeserializeObject<BomberData>(json);
                    break;
            }
        }

        public static int GetViewAmount(this SkillEffectType type)
        {
            switch (type)
            {
                case SkillEffectType.StatModifier:
                    return 1;
                case SkillEffectType.Projectile:
                    return 3;
                case SkillEffectType.AreaDamage:
                    return 1;
                case SkillEffectType.ProjectileDrop:
                    return 2;
                case SkillEffectType.Explosion:
                    return 2;
                case SkillEffectType.ShootingObject:
                    return 3;
                case SkillEffectType.Bomber:
                    return 3;
            }

            return 0;
        }
    }
}