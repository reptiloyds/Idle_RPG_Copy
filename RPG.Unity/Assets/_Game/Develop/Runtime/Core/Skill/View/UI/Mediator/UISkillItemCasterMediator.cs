using System;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main;
using PleasantlyGames.RPG.Runtime.Core.Items.Skill.View;
using PleasantlyGames.RPG.Runtime.Core.Skill.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Skill.View.UI.Mediator
{
    public class UISkillItemCasterMediator : IInitializable, IDisposable
    {
        [Inject] private MainMode _mode;
        [Inject] private ItemSkillService _itemSkillService;
        [Inject] private IWindowService _windowService;
        
        [Preserve]
        public UISkillItemCasterMediator()
        {
            
        }

        public void Initialize() => 
            _itemSkillService.OnEmptySlotInteracted += OnEmptySlotInteracted;

        public void Dispose() => 
            _itemSkillService.OnEmptySlotInteracted -= OnEmptySlotInteracted;

        private async void OnEmptySlotInteracted()
        {
            if(_mode.IsLaunched)
                await _windowService.OpenAsync<SkillInventoryWindow>();
        }
    }
}