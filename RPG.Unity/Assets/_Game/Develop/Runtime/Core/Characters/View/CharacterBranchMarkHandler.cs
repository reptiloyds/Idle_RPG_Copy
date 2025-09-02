using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Branches.View;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.View
{
    public class CharacterBranchMarkHandler : BranchMarkHandler
    {
        [SerializeField, Required] private CharacterWindow _window;
        
        [Inject] private BranchService _branchService;
        [Inject] private CharacterService _characterService;
        
        public void Enable()
        {
            if(!ShowBranchMark) return;
            
            foreach (var view in _window.Views)
            {
                if (!view.Character.IsEquipped) continue;

                var branch = _branchService.GetBranchByCharacter(view.Character.Id);
                if (branch == null) continue;
                
                AddMark(view.transform, branch);
            }
            _characterService.OnSwitched += OnCharacterSwitched;
        }

        public void Disable()
        {
            if(!ShowBranchMark) return;
            ClearAllMarks();
            _characterService.OnSwitched -= OnCharacterSwitched;
        }

        private void OnCharacterSwitched(Character previous, Character current)
        {
            var previousView = _window.GetViewByModel(previous);
            RemoveMark(previousView.transform);
            
            var currentView = _window.GetViewByModel(current);
            var branch = _branchService.GetBranchByCharacter(currentView.Character.Id);
            AddMark(currentView.transform, branch);
        }
    }
}