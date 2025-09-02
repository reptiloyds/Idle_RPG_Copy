using System;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Core.AudioTypes;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main.UI.Mediator
{
    public class UIBossPresentMediator : IInitializable, IDisposable
    {
        [Inject] private MainMode _mainMode;
        [Inject] private IWindowService _windowService;
        [Inject] private IAudioService _audioService;
        
        [Preserve]
        public UIBossPresentMediator()
        {
            
        }
        
        public void Initialize() => 
            _mainMode.OnBossTriggered += OnBossTriggered;

        public void Dispose() => 
            _mainMode.OnBossTriggered -= OnBossTriggered;

        private async void OnBossTriggered()
        {
            await _windowService.OpenAsync<MainGameBossPresentWindow>();
            _audioService.CreateLocalSound(SFX_Common.SFX_Common_BossStart).Play();
        }
    }
}