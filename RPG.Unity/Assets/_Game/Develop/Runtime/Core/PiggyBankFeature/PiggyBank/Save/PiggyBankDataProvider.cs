using PleasantlyGames.RPG.Runtime.Save.Models;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.PiggyBank.Save
{
    public class PiggyBankDataProvider : BaseDataProvider<PiggyBankData>
    {
        public override void LoadData()
        {
            base.LoadData();

            if (Data == null) 
                CreateData();
        }

        private void CreateData()
        {
            Data = new PiggyBankData()
            {
                Level = 1,
            };
        }
    }
}