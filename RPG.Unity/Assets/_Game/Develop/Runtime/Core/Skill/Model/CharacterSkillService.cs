using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.Skill.Type;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model
{
    public class CharacterSkillService : BaseSkillService
    {
        private readonly Dictionary<string, Skill> _skillsDictionary = new ();
        private readonly List<Skill> enabledSkills = new();
        
        [Preserve]
        public CharacterSkillService()
        {
            
        }

        public Skill GetCharacterSkill(string skillId)
        {
            if (_skillsDictionary.TryGetValue(skillId, out var skillModel))
                return skillModel;
            if (Sheet.TryGetValue(skillId, out var skillRow))
                return SkillFactory.Create(skillRow, () => 1);
            Debug.LogError($"Can`t find Skill static data by Id {skillId}");
            return null;
        }

        public override void ResetAllSkill()
        {
            foreach (var skill in enabledSkills)
                skill.ReadyToExecute();
        }

        public override void StopSkills()
        {
            foreach (var skill in enabledSkills) 
                skill.Stop();
        }

        public void EnableCharacterSkill(Skill skill)
        {
            if(ActiveSkills.Contains(skill)) return;
            
            skill.Cooldown();
            ActiveSkills.Add(skill);
            enabledSkills.Add(skill);
        }

        public void DisableCharacterSkill(Skill skill)
        {
            if(!ActiveSkills.Remove(skill)) return;
            
            skill.Stop();
            enabledSkills.Remove(skill);
        }

        protected override void AutoCast()
        {
            foreach (var skill in enabledSkills)
                if (skill.State == SkillState.ReadyToExecute) 
                    skill.Execute();
        }
    }
}