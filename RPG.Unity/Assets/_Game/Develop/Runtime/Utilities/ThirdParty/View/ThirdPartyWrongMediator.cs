using System;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Pause.Contract;
using PleasantlyGames.RPG.Runtime.Pause.Model;
using PleasantlyGames.RPG.Runtime.Pause.Type;
using PleasantlyGames.RPG.Runtime.Utilities.TechnicalMessages.Model;
using PleasantlyGames.RPG.Runtime.Utilities.ThirdParty.Model;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Utilities.ThirdParty.View
{
    public class ThirdPartyWrongMediator : IInitializable, IDisposable
    {
        [Inject] private ThirdPartyEvents _thirdPartyEvents;
        [Inject] private TechnicalMessageService _messageService;
        [Inject] private IPauseService _pauseService;

        [Preserve]
        public ThirdPartyWrongMediator()
        {
            
        }
        
        public void Initialize() => 
            _thirdPartyEvents.OnInitializationFailed += OnInitializationFailed;

        public void Dispose() => 
            _thirdPartyEvents.OnInitializationFailed -= OnInitializationFailed;

        private void OnInitializationFailed(bool handle)
        {
            if (handle)
                _messageService.Open<ThirdPartyWrongView>().Forget();
            _pauseService.Pause(PauseType.Time);
        }
    }
}