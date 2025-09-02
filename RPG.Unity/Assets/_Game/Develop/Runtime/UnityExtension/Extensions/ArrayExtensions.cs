using System;
using Random = UnityEngine.Random;

namespace PleasantlyGames.RPG.Runtime.UnityExtension.Extensions
{
    public static class ArrayExtensions
    {
        public static T GetRandomElement<T>(this T[] list)
        {
            if (list == null || list.Length == 0)
                throw new Exception("You trying to get random element from null or empty array!");
	        
            return list[Random.Range(0, list.Length)];
        }
    }
}