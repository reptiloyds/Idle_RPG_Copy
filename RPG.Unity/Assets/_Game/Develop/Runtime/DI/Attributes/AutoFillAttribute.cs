using System;

namespace PleasantlyGames.RPG.Runtime.DI.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AutoFillAttribute : Attribute
    {
        public bool StrictType { get; private set; }

        public AutoFillAttribute(bool strictType = false)
        {
            StrictType = strictType;
        }
    }
}