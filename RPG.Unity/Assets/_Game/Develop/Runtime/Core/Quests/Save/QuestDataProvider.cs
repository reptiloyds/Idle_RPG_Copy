using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model;
using PleasantlyGames.RPG.Runtime.Save.Models;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Quests.Save
{
    [Serializable]
    public class QuestDataContainer
    {
        public int Id;
        public int Progress;
        
        [UnityEngine.Scripting.Preserve]
        public QuestDataContainer()
        {
        }
    }
    
    public class QuestDataProvider : BaseDataProvider<QuestDataContainer>
    {
        [Inject] private QuestService _questService;
        [UnityEngine.Scripting.Preserve]
        public QuestDataProvider() { }
        
        public override void LoadData()
        {
            base.LoadData();
            
            Data ??= new QuestDataContainer {Id = 1};
            _questService.SetData(Data);
        }
    }
}