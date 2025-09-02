using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Save.Models;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.AndroidRuntime.InApp.Model
{
    [Serializable]
    public class UnityPurchaseDataContainer
    {
        public List<string> NonConfirmedPurchases = new();
        public HashSet<string> RecoverableProducts = new();

        [Preserve]
        public UnityPurchaseDataContainer()
        {
            
        }
    }
    
    public class UnityPurchaseDataProvider : BaseDataProvider<UnityPurchaseDataContainer>
    {
        [Preserve]
        public UnityPurchaseDataProvider() { }
        
        public override void LoadData()
        {
            base.LoadData();

            Data ??= new UnityPurchaseDataContainer();
        }
    }
}