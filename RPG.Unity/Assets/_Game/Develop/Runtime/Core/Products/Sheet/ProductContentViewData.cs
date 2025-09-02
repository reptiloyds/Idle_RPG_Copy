using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Sheet
{
    [Serializable]
    public class ProductContentViewData
    {
        public bool UseProductImages;
        public List<string> ManualImages;
        
        [Preserve]
        public ProductContentViewData()
        {
            
        }
    }
}