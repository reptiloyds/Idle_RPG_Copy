using System;
using Cathei.BakingSheet.Internal;
using UnityEngine.Serialization;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.CloseWindow
{
    [Serializable]
    public class CloseWindowData
    {
        public string WindowId;
        
        [Preserve]
        public CloseWindowData()
        {
        }
    }
}