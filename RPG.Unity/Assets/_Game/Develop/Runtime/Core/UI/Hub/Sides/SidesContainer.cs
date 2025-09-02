using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Hub.Sides
{
    public class SidesContainer
    {
        private readonly Dictionary<UISideType, RectTransform> _dictionary = new();
        
        [Preserve]
        public SidesContainer()
        {
        }
        
        public void RegisterSide(UISideType type, RectTransform transform) => 
            _dictionary[type] = transform;

        public RectTransform GetSide(UISideType type) =>
            _dictionary[type];
    }
}