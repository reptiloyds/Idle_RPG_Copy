using System;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers.Content
{
    [Serializable]
    public class ContentTutorialData
    {
        public string Id;
        
        [Preserve]
        public ContentTutorialData()
        {
        }
    }
}