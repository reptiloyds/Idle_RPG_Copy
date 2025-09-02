using System;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Stats.Sheet
{
    public struct ManualStatData<T> where T : Enum
    {
        public T Type;
        public double M;
        public long E;
    }
}