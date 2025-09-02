using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Extensions
{
    public static class ColorExtensions
    {
        public static string ToHexString(this Color color)
        {
            int r = Mathf.RoundToInt(color.r * 255);
            int g = Mathf.RoundToInt(color.g * 255);
            int b = Mathf.RoundToInt(color.b * 255);
            
            return $"#{r:X2}{g:X2}{b:X2}";
        }
    }
}