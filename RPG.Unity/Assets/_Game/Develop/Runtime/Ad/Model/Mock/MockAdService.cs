using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Ad.Model.Mock
{
    public sealed class MockAdService : BaseAdService
    {
        [Preserve]
        public MockAdService()
        {
        }
        
        public override void ShowReward(string id)
        {
            base.ShowReward(id);
            RewardResult(true);
        }

        protected override bool IsRewardAdReady() => 
            true;
    }
}