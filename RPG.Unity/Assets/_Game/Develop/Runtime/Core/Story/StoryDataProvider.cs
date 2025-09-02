using System;
using PleasantlyGames.RPG.Runtime.Save.Models;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Story
{
    [Serializable]
    public class StoryDataContainer
    {
        public bool Completed;
        
        [Preserve]
        public StoryDataContainer() { }
    }
    
    public class StoryDataProvider :  BaseDataProvider<StoryDataContainer>
    {
        [Preserve]
        public StoryDataProvider() { }

        public override void LoadData()
        {
            base.LoadData();
            
            Data ??= new ();
        }
    }
}