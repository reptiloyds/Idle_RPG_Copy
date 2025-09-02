using System;
using Sirenix.OdinInspector;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.Model.Definition
{
    [Serializable]
    public class SkillConfiguration
    {
        [MinValue(0)] public float AutoCastDelay = 0.5f;
    }
}