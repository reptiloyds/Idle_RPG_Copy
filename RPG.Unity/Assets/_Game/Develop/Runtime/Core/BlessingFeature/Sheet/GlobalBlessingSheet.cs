using System;
using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Type;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Sheet
{
    public class GlobalBlessingSheet : Sheet<string, GlobalBlessingSheet.Row>
    {
        [Preserve] public GlobalBlessingSheet() { }
        
        public class Row : BlessingRow
        {
            [Preserve] public Row() { }
        }
    }
}