using System;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.Pointer
{
    [Serializable]
    public class PointerTutorialData
    {
        public string ButtonId;
        public string Sprite;
        
        [Preserve]
        public PointerTutorialData()
        {
        }
    }
}