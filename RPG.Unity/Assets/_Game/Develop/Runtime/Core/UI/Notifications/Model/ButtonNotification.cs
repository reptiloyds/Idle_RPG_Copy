using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Contract;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Contract;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.View;
using PleasantlyGames.RPG.Runtime.Pool;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model
{
    public class ButtonNotification : INotificationProvider
    {
        [Inject] private IButtonService _buttonService;

        private BaseButton _button;
        private Notification _notification;
        private readonly NotificationSetup _setup;
        private readonly string _buttonId;

        private readonly ObjectPoolWithParent<NotificationView> _pool;
        private readonly List<Notification> _children = new();
        private readonly List<INotificationProvider> _providers = new();
        
        public event Action<INotificationProvider> OnMainNotificationChanged;

        public ButtonNotification(string buttonId, NotificationSetup setup, ObjectPoolWithParent<NotificationView> pool)
        {
            _buttonId = buttonId;
            _setup = setup;
            _pool = pool;
        }

        public ButtonNotification(BaseButton button, NotificationSetup setup, ObjectPoolWithParent<NotificationView> pool)
        {
            _button = button;
            _setup = setup;
            _pool = pool;
        }

        public void Initialize()
        {
            _notification = new Notification(_pool, _setup);
            
            if (_button == null) 
                _button = _buttonService.GetButton(_buttonId);
            
            if (_button == null) 
                _buttonService.OnButtonRegistered += OnButtonRegistered;
            else
                AddParent(_button.transform);
        }

        public void Dispose()
        {
            if (_button == null) 
                _buttonService.OnButtonRegistered -= OnButtonRegistered;
        }

        public void AddProvider(INotificationProvider provider)
        {
            _providers.Add(provider);
            provider.OnMainNotificationChanged += OnNotificationChanged;
            ChangeChildren();
        }

        public void AddProviders(List<INotificationProvider> providers)
        {
            _providers.AddRange(providers);
            foreach (var provider in providers) 
                provider.OnMainNotificationChanged += OnNotificationChanged;
            ChangeChildren();
        }

        private void OnButtonRegistered(BaseButton button)
        {
            if(!string.Equals(button.Id, _buttonId)) return;
            _buttonService.OnButtonRegistered -= OnButtonRegistered;
            AddParent(button.transform);
        }

        private void OnNotificationChanged(INotificationProvider changedProvider) => 
            ChangeChildren();

        private void ChangeChildren()
        {
            _notification.ClearChild();
            _children.Clear();
            foreach (var provider in _providers) 
                provider.FillMainNotifications(_children);

            foreach (var child in _children) 
                _notification.AppendChild(child);   
        }

        private void AddParent(Transform transform) => 
            _notification.SetParent(transform);

        public void FillMainNotifications(in List<Notification> notifications) => 
            notifications.Add(_notification);
    }
}