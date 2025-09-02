using System;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model.Bonus;
using PleasantlyGames.RPG.Runtime.Core.Characters.Sheet.Data;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PrimeTween;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.View.Mediator
{
    public class UICharacterUpMediator : IInitializable, IDisposable
    {
        [Inject] private CharacterService _characterService;
        [Inject] private IWindowService _windowService;
        
        [Preserve]
        public UICharacterUpMediator()
        {
        }
        
        public void Initialize() => 
            _characterService.OnBonusUp += OnBonusUp;

        public void Dispose() => 
            _characterService.OnBonusUp -= OnBonusUp;
        
        private async void OnBonusUp(Character character, ICharacterBonus characterBonus)
        {
            if (characterBonus.ConditionType == BonusConditionType.Evolution)
            {
                var evolutionAnimationWindow = await _windowService.OpenAsync<CharacterEvolutionAnimationWindow>();
                evolutionAnimationWindow.Setup(character);

                Tween.Delay(evolutionAnimationWindow.AnimationDuration, Complete);

                async void Complete()
                {
                    _windowService.Close<CharacterEvolutionAnimationWindow>();
                    var window = await _windowService.OpenAsync<CharacterUpWindow>();
                    window.Setup(character, characterBonus);
                }
            }
            else
            {
                var window = await _windowService.OpenAsync<CharacterUpWindow>();
                window.Setup(character, characterBonus);
            }
        }
    }
}