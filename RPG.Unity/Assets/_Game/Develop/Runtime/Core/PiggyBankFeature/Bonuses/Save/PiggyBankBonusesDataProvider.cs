using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Data;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Sheets;
using PleasantlyGames.RPG.Runtime.Save.Models;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Save
{
    public class PiggyBankBonusesDataProvider : BaseDataProvider<PiggyBankBonusesDataContainer>
    {
        [Inject] private BalanceContainer _balance;
        
        public override void LoadData()
        {
            base.LoadData();

            if (Data == null)
                CreateData();
            else
                ValidateData();
        }

        private void CreateData()
        {
            var sheet = _balance.Get<PiggyBankBonusesSheet>();
            Data = new PiggyBankBonusesDataContainer();

            foreach (var row in sheet) 
                AddData(row);
        }

        private void ValidateData()
        {
            var sheet = _balance.Get<PiggyBankBonusesSheet>();

            foreach (var row in sheet)
            {
                if(HasWithId(row.Id)) continue;
                AddData(row);
            }
        }

        private void AddData(PiggyBankBonusesSheet.PiggyBankBonusRow config)
        {
            Data.Bonuses.Add(new PiggyBankBonusData()
            {
                Id = config.Id,
                IsCollected = false,
            });
        }

        private bool HasWithId(string id)
        {
            foreach (var data in Data.Bonuses)
                if (string.Equals(data.Id, id)) return true;

            return false;
        }
    }
}