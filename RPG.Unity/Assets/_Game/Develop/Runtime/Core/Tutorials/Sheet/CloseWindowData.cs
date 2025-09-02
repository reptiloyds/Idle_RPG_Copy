using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Sheet
{
    [Serializable]
    public class CloseWindowData
    {
        public bool Close;
        public HashSet<string> ExcludeIds = new(0);
        public HashSet<string> Ids = new(0);
        
        [Preserve]
        public CloseWindowData()
        {
        }
    }
}