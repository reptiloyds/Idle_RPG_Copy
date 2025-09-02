using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.View;
using PleasantlyGames.RPG.Runtime.Pool;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Model
{
    public class Notification
    {
        private readonly ObjectPoolWithParent<NotificationView> _pool;
        private readonly NotificationSetup _setup;
        private NotificationView _view;
        private Transform _parent;
        private readonly NotificationImageSetup _imageSetup;

        private readonly List<Notification> _children = new();

        public NotificationImageSetup ImageSetup => _imageSetup ?? GetChildImageSetup();
        public bool IsActive { get; private set; }

        public event Action<Notification> OnStateChanged;

        public Notification(ObjectPoolWithParent<NotificationView> pool, NotificationSetup setup, NotificationImageSetup imageSetup = null)
        {
            _pool = pool;
            _setup = setup;
            _imageSetup = imageSetup;
        }

        public void AppendChild(Notification child)
        {
            _children.Add(child);
            child.OnStateChanged += OnChildStateChanged;
            UpdateByChildren();
        }

        public void ClearChild()
        {
            foreach (var child in _children) 
                child.OnStateChanged -= OnChildStateChanged;
        }

        private NotificationImageSetup GetChildImageSetup()
        {
            Notification anyActiveNotification = null;
            foreach (var child in _children)
            {
                if (!child.IsActive) continue;
                if (child.ImageSetup.ImageType == NotificationImageType.Default)
                    return child.ImageSetup;
                anyActiveNotification = child;
            }
            
            return anyActiveNotification!.ImageSetup;
        }

        private void OnChildStateChanged(Notification childNotification) => 
            UpdateByChildren();

        private void UpdateByChildren()
        {
            bool isAnyChildActive = false;
            foreach (var child in _children)
            {
                if (!child.IsActive) continue;
                isAnyChildActive = true;
                break;
            }

            if (isAnyChildActive)
            {
                if (!IsActive)
                    Enable();
                else if (_view != null) 
                    _view.SetImage(ImageSetup);
            }
            else
                Disable();   
        }

        public void SetParent(Transform parent)
        {
            _parent = parent;
            if (!IsActive) return;
            
            if (_view != null)
            {
                _pool.Release(_view);
                _view = null;
            } 
            SpawnInParent();
        }

        public void RemoveParent()
        {
            _parent = null;
            
            if (_view == null) return;
            _pool.Release(_view);
            _view = null;
        }

        public void Enable()
        {
            if(IsActive) return;
            IsActive = true;

            if (_parent != null) 
                SpawnInParent();
            
            OnStateChanged?.Invoke(this);
        }

        public void Disable()
        {
            if(!IsActive) return;
            IsActive = false;

            if (_view != null)
            {
                _pool.Release(_view);
                _view = null;   
            }
            
            OnStateChanged?.Invoke(this);
        }

        private void SpawnInParent()
        {
            _view = _pool.Get();
            _view.Setup(_parent, _setup);
            _view.SetImage(ImageSetup);
        }
    }
}