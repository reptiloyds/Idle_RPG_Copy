using System;

namespace PleasantlyGames.RPG.Runtime.Core.Balance.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class SheetListAttribute : Attribute
    {
        public string SheetName { get; }

        public SheetListAttribute(string sheetName) => 
            SheetName = sheetName;
    }
}