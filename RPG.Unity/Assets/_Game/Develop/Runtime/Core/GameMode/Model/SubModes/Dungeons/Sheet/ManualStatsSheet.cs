using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Stats.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Sheet
{
    public class ManualStatsSheet<T> : Sheet<int, ManualStatsSheet<T>.Row> where T : class
    {
        [Preserve]
        public ManualStatsSheet() { }
        
        public class Row : SheetRowArray<int, Elem>
        {
            [Preserve]
            public Row() { }
        } 
        
        
        public class Elem : SheetRowElem
        {
            [Preserve] public UnitStatType Type { get; private set; }
            [Preserve] public double Mantissa { get; private set; }
            [Preserve] public long Exponent { get; private set; }

            public ManualStatData<UnitStatType> ManualStatData;
            
            [Preserve]
            public Elem() { }

            public override void PostLoad(SheetConvertingContext context)
            {
                base.PostLoad(context);
                ManualStatData = new ManualStatData<UnitStatType>()
                {
                    Type = Type,
                    M = Mantissa,
                    E = Exponent
                };
            }
        }
    }
}