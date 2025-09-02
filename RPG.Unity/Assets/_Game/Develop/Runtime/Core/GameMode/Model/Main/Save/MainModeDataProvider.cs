using System;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Save.Models;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main.Save
{
    [Serializable]
    public class MainModeDataContainer
    {
        public int Id;
        public int Level;
        public bool WavesIsCleared;
        
        [UnityEngine.Scripting.Preserve]
        public MainModeDataContainer()
        {
        }
    }
    
    public class MainModeDataProvider : BaseDataProvider<MainModeDataContainer>
    {
        [Inject] private MainMode _mode;
        [Inject] private BalanceContainer _balance;

        [UnityEngine.Scripting.Preserve]
        public MainModeDataProvider() { }

        public override void LoadData()
        {
            base.LoadData();

            Data ??= new MainModeDataContainer()
            {
                Level = 1,
                WavesIsCleared = false
            };
            _mode.SetData(Data);
        }
    }
}