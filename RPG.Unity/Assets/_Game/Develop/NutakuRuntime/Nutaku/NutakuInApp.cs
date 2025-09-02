using System;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.NutakuRuntime.Nutaku
{
    [Serializable]
    public class NutakuInApp
    {
        public string Id;
        public int Price;
        public string FormattedName;
        public string FormattedDescription;
        public string ImageUrl;
        
        [Preserve]
        public NutakuInApp() { }
    }
}