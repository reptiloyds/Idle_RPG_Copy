using System.Collections.Generic;

namespace PleasantlyGames.RPG.Runtime.Utilities.Extensions
{
    public static class ReadOnlyListExtensions
    {
        public static int IndexOf<T>(this IReadOnlyList<T> list, T value)
        {
            for (int i = 0; i < list.Count; i++)
                if (EqualityComparer<T>.Default.Equals(list[i], value))
                    return i;
            return -1;
        }
    }

}