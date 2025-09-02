using Cathei.BakingSheet;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Sheet
{
    public class VerticalProductVisualSheet : Sheet<string, VerticalVisualRow>
    {
        [Preserve]
        public VerticalProductVisualSheet() { }
    }
    
    public class VerticalVisualRow : ProductVisualRow
    {
        [Preserve]
        public VerticalVisualRow() { }
    }
}