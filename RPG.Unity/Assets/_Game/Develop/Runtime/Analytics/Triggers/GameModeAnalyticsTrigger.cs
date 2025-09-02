using System;
using PleasantlyGames.RPG.Runtime.Analytics.Contract;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Analytics.Triggers
{
    public class GameModeAnalyticsTrigger : IInitializable, IDisposable
    {
        [Inject] private IAnalyticsService _analytics;
        [Inject] private MainMode _mainMode;
        
        public void Initialize()
        {
            _mainMode.OnLevelChanged += SendLevelChangedEvent;
        }

        public void Dispose()
        {
            _mainMode.OnLevelChanged -= SendLevelChangedEvent;
        }

        private void SendLevelChangedEvent()
        {
            _analytics.SendLevelChanged(_mainMode.Id, _mainMode.Level);
        }
    }
}