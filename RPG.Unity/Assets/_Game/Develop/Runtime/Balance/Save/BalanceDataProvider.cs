using System;
using PleasantlyGames.RPG.Runtime.Balance.Const;
using PleasantlyGames.RPG.Runtime.Save.Models;

namespace PleasantlyGames.RPG.Runtime.Balance.Save
{
    [Serializable]
    public class BalanceData
    {
        [UnityEngine.Scripting.Preserve]
        public BalanceData()
        {
        }

        public string FileName;
    }
    
    public class BalanceDataProvider : BaseDataProvider<BalanceData>
    {
        [UnityEngine.Scripting.Preserve]
        public BalanceDataProvider()
        {
        }

        public override void LoadData()
        {
            base.LoadData();

            if (Data == null) 
                CreateData();
        }
        
        private void CreateData()
        {
            Data = new BalanceData
            {
                FileName = BalanceConst.DefaultFileName
            };
        }
    }
}