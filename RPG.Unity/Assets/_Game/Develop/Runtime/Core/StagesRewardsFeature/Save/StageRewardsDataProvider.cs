using PleasantlyGames.RPG.Runtime.Save.Models;

namespace PleasantlyGames.RPG.Runtime.Core.StagesRewardsFeature.Save
{
    public class StageRewardsDataProvider : BaseDataProvider<StageRewardsData>
    {
        public override void LoadData()
        {
            base.LoadData();
            
            if (Data == null)
                Data = new StageRewardsData();
        }
    }
}