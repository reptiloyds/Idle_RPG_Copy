using System;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.Monologue
{
    [Serializable]
    public class MonologueTutorialData
    {
        public string Position;
        public string MessageToken;
        public bool Background;
        public float BackgroundAlpha = -1;
        
        [Preserve]
        public MonologueTutorialData()
        {
        }
    }
}