using System;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Utilities.Extensions;
using Random = UnityEngine.Random;

namespace PleasantlyGames.RPG.Runtime.UnityExtension
{
    public static class ListExtensions
    {
        public static T Next<T>(this IReadOnlyList<T> list, int index) where T : class => 
            index + 1 < list.Count ? list[index + 1] : null;
        
        public static bool IsLast<T>(this IReadOnlyList<T> list, int index) => 
            index + 1 >= list.Count;

        public static T GetRandomElement<T>(this List<T> list)
        {
            if (list == null || list.Count == 0)
                throw new Exception("You trying to get random element from null or empty list!");
	        
            return list[Random.Range(0, list.Count)];
        }

        public static T WeightedRandomElement<T>(this List<T> list) where T : IWeightedRandom
        {
            var sum = 0;
            var random = Random.Range(0, list.Sum(item => item.RandomWeight) + 1);
            foreach (var element in list)
            {
                sum += element.RandomWeight;
                if (sum >= random) 
                    return element;
            }

            return list[0];
        }

        private static System.Random rng = new();  
        
        public static void Shuffle<T>(this IList<T> list)  
        {  
            var n = list.Count;  
            while (n > 1) {  
                n--;  
                var k = rng.Next(n + 1);  
                (list[k], list[n]) = (list[n], list[k]);
            }  
        }
    }

    public interface IWeightedRandom
    {
        int RandomWeight { get; }
    }
}