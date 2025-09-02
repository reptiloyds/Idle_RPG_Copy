using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Ad.Contracts;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Config;
using PleasantlyGames.RPG.Runtime.Core.UI.Notifications.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.Tween;
using PrimeTween;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Notifications.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class NotificationView : MonoBehaviour
    {
        [Serializable]
        private class AnimationSettings
        {
            public NotificationImageType Type;
            public TweenSettings<Vector3> Settings;
        }
        
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Sprite _adSprite;
        [SerializeField] private Sprite _freeAdSprite;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private UIScale _scale;
        [SerializeField] private List<AnimationSettings> _animationSettings;
        [SerializeField] private bool _setupOnAwake;
        [SerializeField] [HideIf("@this._setupOnAwake == false")]
        private NotificationImageSetup _defaultSetup;

        private NotificationImageSetup _currentSetup;
        
        private IAdService _adService;

        private void Awake()
        {
            if (_setupOnAwake) 
                SetImage(_defaultSetup);
        }

        public void Initialize(IAdService adService)
        {
            _adService = adService;
            if (!_adService.IsDisabled.CurrentValue)
            {
                _adService.IsDisabled
                    .Subscribe(value => RedrawSprite())
                    .AddTo(this);
            }
        }

        public void Setup(Transform parent, NotificationSetup setup)
        {
            transform.SetParent(parent);
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;

            _rectTransform.anchorMin = setup.AnchorMin;
            _rectTransform.anchorMax = setup.AnchorMax;
            _rectTransform.anchoredPosition = setup.AnchoredPosition;
        }

        public void SetImage(NotificationImageSetup setup)
        {
            _currentSetup = setup;
            RedrawSprite();

            foreach (var setting in _animationSettings)
            {
                if (setting.Type == _currentSetup.ImageType)
                {
                    _scale.SetSettings(setting.Settings);
                    break;
                }
            }
        }

        private void RedrawSprite()
        {
            switch (_currentSetup.ImageType)
            {
                case NotificationImageType.Default:
                    _image.sprite = _defaultSprite;
                    _rectTransform.sizeDelta = _currentSetup.Size;
                    break;
                case NotificationImageType.Ad:
                    if (_adService.IsDisabled.CurrentValue)
                    {
                        _rectTransform.sizeDelta = _defaultSetup.Size;
                        _image.sprite = _freeAdSprite;
                    }
                    else
                    {
                        _rectTransform.sizeDelta = _currentSetup.Size;
                        _image.sprite = _adSprite;
                    }
                    break;
                default:
                    _image.sprite = null;
                    _rectTransform.sizeDelta = _currentSetup.Size;
                    break;
            }
        }
    }
}