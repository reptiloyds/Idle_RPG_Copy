using System;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers.MainMode
{
    [Serializable]
    internal class MainModeEnterData
    {
        public int Id;
        public int Level;
        
        [Preserve]
        public MainModeEnterData()
        {
        }
    }
}