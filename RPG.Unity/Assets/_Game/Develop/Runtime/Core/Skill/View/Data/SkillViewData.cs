using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.View.Data
{
    [Serializable]
    public class SkillViewData
    {
        public string Key;
        public float Size;
        public float X;
        public float Y;
        public float Z;

        public Vector3 GetSize()
        {
            if (Z != 0 || X != 0 || Y != 0)
                return new Vector3(X, Y, Z);
            return Size == 0 ? Vector3.one : new Vector3(Size, Size, Size);
        }
        
        [Preserve]
        public SkillViewData()
        {
        }
    }
}