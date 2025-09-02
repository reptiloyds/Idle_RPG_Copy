using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Contract;
using UnityEngine.Scripting;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.TutorialAnimation
{
    public class TutorialAnimationLauncher
    {
        private readonly Dictionary<string, TutorialAnimation> _activeBuffer = new();
        
        [Inject] private IButtonService _buttonService;
        
        [UnityEngine.Scripting.Preserve]
        public TutorialAnimationLauncher()
        {
        }
        
        public void Play(string buttonId, Action callback)
        {
            if (!_buttonService.IsButtonRegistered(buttonId))
                return;
            
            var button = _buttonService.GetButton(buttonId);
            
            if (button.TryGetComponent(out TutorialAnimation tutorialAnimation))
            {
                _activeBuffer.Add(buttonId, tutorialAnimation);
                tutorialAnimation.Play(callback, CompleteAnimationCallback);
            }
        }

        public void Stop(string buttonId)
        {
            if (_activeBuffer.Remove(buttonId, out var tutorialAnimation))
                tutorialAnimation.Stop();
        }

        private void CompleteAnimationCallback(TutorialAnimation tutorialAnimation)
        {
            foreach (var kvp in _activeBuffer)
            {
                if (kvp.Value != tutorialAnimation) continue;
                _activeBuffer.Remove(kvp.Key);
                break;
            }
        }
    }
}