using Cathei.BakingSheet;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Sheet
{
    public class BlessingSheet : Sheet<string, BlessingSheet.Row>
    {
        [Preserve] public BlessingSheet() { }
        
        public class Row : BlessingRow
        {
            [Preserve] public int FreeActivation { get; private set; }
            [Preserve] public float DurationInMinute { get; private set; }
            [Preserve] public bool Increase { get; private set; }

            [Preserve] public Row() { }
        }
    }
}