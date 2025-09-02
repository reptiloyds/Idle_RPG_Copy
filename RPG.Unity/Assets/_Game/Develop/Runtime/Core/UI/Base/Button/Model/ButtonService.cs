using System;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Contract;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Pause.Contract;
using PleasantlyGames.RPG.Runtime.Pause.Type;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model
{
    public sealed class ButtonService : IButtonService, IInitializable, IDisposable
    {
        private readonly Dictionary<string, List<BaseButton>> _buttons = new();
        private readonly HashSet<string> _tutorialButtons = new();
        
        public event Action<BaseButton> OnButtonClick;
        public event Action<string> OnButtonIdClick;
        public event Action<BaseButton> OnButtonRegistered;

        public bool IsButtonInputBlocked { get; private set; }

        private readonly IPauseService _pauseService;

        [UnityEngine.Scripting.Preserve]
        [Inject]
        public ButtonService(IPauseService pauseService) => 
            _pauseService = pauseService;

        public void Initialize()
        {
            IsButtonInputBlocked = _pauseService.IsPauseEnabled(PauseType.UIInput);
            _pauseService.OnPause += OnPause;
            _pauseService.OnContinue += OnContinue;
        }

        public void Dispose()
        {
            _pauseService.OnPause -= OnPause;
            _pauseService.OnContinue -= OnContinue;
        }

        private void OnPause(PauseType pauseType)
        {
            if(!pauseType.HasFlag(PauseType.UIInput)) return;
            IsButtonInputBlocked = true;
        }

        private void OnContinue(PauseType pauseType)
        {
            if(!pauseType.HasFlag(PauseType.UIInput)) return;
            IsButtonInputBlocked = false;
        }

        public void RegisterButton(BaseButton button)
        {
            if(string.IsNullOrEmpty(button.Id)) return;
            if (!_buttons.TryGetValue(button.Id, out var list))
            {
                list = new List<BaseButton>(2);
                _buttons.Add(button.Id, list);
            }
            list.Add(button);
            OnButtonRegistered?.Invoke(button);
        }

        public void UnregisterButton(BaseButton button)
        {
            if(string.IsNullOrEmpty(button.Id)) return;
            if (_buttons.TryGetValue(button.Id, out var list)) 
                list.Remove(button);
        }

        public BaseButton GetButton(string id)
        { 
            var button = _buttons.GetValueOrDefault(id);
            if (button == null)
            {
                Debug.LogError($"Can`t find button wih Id {id}");
                return null;
            }
            return button.FirstOrDefault();
        }

        public void AppendAllowedButtonId(string id) => 
            _tutorialButtons.Add(id);

        public void AppendAllowedButtonIds(List<string> ids)
        {
            foreach (var id in ids) 
                AppendAllowedButtonId(id);
        }

        public void RemoveAllowedButtonId(string id) => 
            _tutorialButtons.Remove(id);

        public void RemoveAllowedButtonIds(List<string> ids)
        {
            foreach (var id in ids) 
                RemoveAllowedButtonId(id);
        }

        public bool IsAllowedButton(string id) => 
            _tutorialButtons.Contains(id);

        public bool IsButtonRegistered(string id)
        {
            if (_buttons.TryGetValue(id, out var list))
                return list.Count > 0;
            return false;
        }

        public void TriggerButtonClick(BaseButton button)
        {
            OnButtonClick?.Invoke(button);
            if(string.IsNullOrEmpty(button.Id)) return;
            OnButtonIdClick?.Invoke(button.Id);
        }
    }
}