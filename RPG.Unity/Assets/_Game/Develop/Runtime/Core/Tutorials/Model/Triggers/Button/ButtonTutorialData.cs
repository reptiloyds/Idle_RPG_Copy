using System;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers.Button
{
    [Serializable]
    public class ButtonTutorialData
    {
        public string ButtonId;
        
        [Preserve]
        public ButtonTutorialData()
        {
        }
    }
}