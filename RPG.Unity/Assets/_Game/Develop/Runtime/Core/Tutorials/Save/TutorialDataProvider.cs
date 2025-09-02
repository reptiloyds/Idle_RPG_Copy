using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Save.Models;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Save
{
    [Serializable]
    public class TutorialDataContainer
    {
        public HashSet<string> CompletedTutorials = new();
        public string CurrentTutorialId;
        public int CurrentStepOrder;
        
        [UnityEngine.Scripting.Preserve]
        public TutorialDataContainer()
        {
        }
    }
    
    public class TutorialDataProvider : BaseDataProvider<TutorialDataContainer>
    {
        [UnityEngine.Scripting.Preserve]
        public TutorialDataProvider() { }
        
        public override void LoadData()
        {
            base.LoadData();
            
            Data ??= new TutorialDataContainer();
        }
    }
}