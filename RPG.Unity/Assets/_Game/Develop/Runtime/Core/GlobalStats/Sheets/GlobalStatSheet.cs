using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Type;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.GlobalStats.Sheets
{
    public class GlobalStatSheet : Sheet<GlobalStatType, GlobalStatSheet.Row>
    {
        [Preserve] public GlobalStatSheet() { }
        
        public class Row : SheetRow<GlobalStatType>
        {
            [Preserve] public float Value { get; private set; }
            
            [Preserve] public Row() { }
        }
    }
}