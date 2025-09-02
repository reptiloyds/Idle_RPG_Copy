using System;
using PleasantlyGames.RPG.Runtime.Ad.Contracts;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Ad.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class AdVisualSwitcher : MonoBehaviour
    {
        [Serializable]
        private class ImageSetup
        {
            public Image Image;
            public Sprite EnableState;
            public Sprite DisableState;

            public void ApplyEnableState() => 
                Image.sprite = EnableState;

            public void ApplyDisableState() => 
                Image.sprite = DisableState;
        }
        [Serializable]
        private class ColorSetup
        {
            public Graphic Graphic;
            public Color EnableColor;
            public Color DisableColor;
            
            public void ApplyEnableState() => 
                Graphic.color = EnableColor;

            public void ApplyDisableState() => 
                Graphic.color = DisableColor;
        }
        
        [SerializeField] private GameObject[] _enableStateObjects;
        [SerializeField] private ImageSetup[] _imageSetups;
        [SerializeField] private ColorSetup[] _colorSetups;

        [Inject] private IAdService _adService;

        private void Awake()
        {
            if(_adService == null) return;

            if (!_adService.IsDisabled.CurrentValue)
            {
                _adService.IsDisabled.Subscribe(value => RedrawState())
                    .AddTo(this);
            } 
            
            RedrawState();
        }

        private void RedrawState()
        {
            foreach (var enableStateObject in _enableStateObjects) 
                enableStateObject.SetActive(!_adService.IsDisabled.CurrentValue);
            foreach (var imageSetup in _imageSetups)
            {
                if (_adService.IsDisabled.CurrentValue) 
                    imageSetup.ApplyDisableState();
                else
                    imageSetup.ApplyEnableState();
            }

            foreach (var colorSetup in _colorSetups)
            {
                if(_adService.IsDisabled.CurrentValue)
                    colorSetup.ApplyDisableState();
                else
                    colorSetup.ApplyEnableState();
            }
        }
    }
}