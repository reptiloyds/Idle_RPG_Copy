using System;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.OpenWindow
{
    [Serializable]
    public class OpenWindowData
    {
        public string WindowId;
        
        [Preserve]
        public OpenWindowData()
        {
        }
    }
}