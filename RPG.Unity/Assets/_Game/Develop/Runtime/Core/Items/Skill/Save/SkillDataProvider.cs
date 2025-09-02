using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Save;
using PleasantlyGames.RPG.Runtime.Save.Models;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Skill.Save
{
    [Serializable]
    public class SkillDataContainer
    {
        public bool IsAutoCastEnabled;
        public List<SkillData> List = new();
        
        [UnityEngine.Scripting.Preserve]
        public SkillDataContainer()
        {
        }
    }

    [Serializable]
    public class SkillData : SlotData
    {
        public int SlotId;
        
        [Cathei.BakingSheet.Internal.Preserve]
        public SkillData()
        {
            
        }
    }
    
    public class SkillDataProvider : BaseDataProvider<SkillDataContainer>
    {
        private const int SKILL_SLOTS = 6;

        [UnityEngine.Scripting.Preserve]
        public SkillDataProvider() { }

        public override void LoadData()
        {
            base.LoadData();

            if (Data == null)
                CreateData();
        }

        private void CreateData()
        {
            Data = new SkillDataContainer();
            var slotAmount = SKILL_SLOTS;
            
            for (var i = 0; i < slotAmount; i++)
                Data.List.Add(new SkillData()
                {
                    SlotId = i,
                    ItemId = string.Empty
                });
        }
    }
}