using PleasantlyGames.RPG.Runtime.Core.Stats.Model;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;

namespace PleasantlyGames.RPG.Runtime.Core.Extensions
{
    public static class StatExtensions
    {
        private const string StatSign = "+";
        
        public static string GetFormattedModifier(this StatModType modType, BigDouble.Runtime.BigDouble value, bool useSign = true)
        {
            switch (modType)
            {
                case StatModType.Flat:
                    return $"{(useSign ? StatSign : string.Empty)}{StringExtension.Instance.CutBigDouble(value)}";
                case StatModType.GroupKAdditive :
                case StatModType.KAdditive:
                case StatModType.PercentAdd:
                case StatModType.PercentMult:
                    value *= 100;
                    return $"{(useSign ? StatSign : string.Empty)}{StringExtension.Instance.CutBigDouble(value)}%";
                default:
                    return string.Empty;
            }
        }
        
        public static bool ReplaceModifier(this BaseStat stat, StatModifier oldMod, StatModifier newMod)
        {
            var result = stat.RemoveModifier(oldMod);
            stat.AddModifier(newMod);
            return result;
        }
    }
}