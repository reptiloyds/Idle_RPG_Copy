using PleasantlyGames.RPG.Runtime.Analytics.Contract;
using PleasantlyGames.RPG.Runtime.Save.Models;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Analytics.Save
{
    public class AnalyticsDataProvider : BaseDataProvider<AnalyticsData>
    {
        [Inject] private IAnalyticsService _analytics;
        
        [Preserve]
        public AnalyticsDataProvider()
        {
        }
        
        public override void LoadData()
        {
            base.LoadData();

            if (Data == null)
            {
                Data = new AnalyticsData();
            }

            _analytics.SetData(Data);
        }
    }
}