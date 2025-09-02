using System;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Building;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.Effects
{
    [Serializable]
    public class RenderGroup
    {
        public List<RenderConfig> Configs;

        public void Initialize()
        {
            foreach (var config in Configs) 
                config.Initialize();
        }
        
        public void SetOriginal()
        {
            foreach (var config in Configs) 
                config.SetOriginal();
        }

        public void SetBlink()
        {
            foreach (var config in Configs) 
                config.SetBlink();
        }
    }
    
    [Serializable]
    public class RenderConfig
    {
        private enum BlinkType
        {
            SwitchMaterial = 0,
            SwitchProperty = 1,
        }
        
        [SerializeField] private List<Renderer> _renderers;
        [SerializeField] private BlinkType _blinkType;
        
        [SerializeField, HideIf("@this._blinkType != BlinkType.SwitchMaterial")]
        private Material _originalMaterial;
        [SerializeField, HideIf("@this._blinkType != BlinkType.SwitchMaterial")]
        private Material _blinkMaterial;

        [SerializeField, MinValue(0), HideIf("@this._blinkType != BlinkType.SwitchProperty")]
        private int _materialId = 0;
        [SerializeField, HideIf("@this._blinkType != BlinkType.SwitchProperty")] private bool _switchFloat = true;
        [SerializeField, HideIf("@this._blinkType != BlinkType.SwitchProperty")] private bool _switchColor = true;
        [SerializeField, HideIf("@this._blinkType != BlinkType.SwitchProperty || this._switchFloat == false")]
        private string _propertyName = "_Emission";
        [SerializeField, HideIf("@this._blinkType != BlinkType.SwitchProperty || this._switchFloat == false")]
        private float _blinkPropertyValue = 5;
        [SerializeField, HideIf("@this._blinkType != BlinkType.SwitchProperty || this._switchColor == false")]
        private string _propertyColorName = "_BaseColor";
        [SerializeField, HideIf("@this._blinkType != BlinkType.SwitchProperty || this._switchColor == false")]
        private Color _propertyColorValue = Color.white;
        
        private float DefaultEmissionValue;
        private Color DefaultEmissionColor;

        private MaterialPropertyBlock _mpb;

        public void LogIfWrong(ref int errorCount, GameObject gameObject = null)
        {
            if (_renderers.Count == 0 || _renderers.Any(item => item == null))
            {
                errorCount++;
                Logger.LogError($"Renderers are not set on {nameof(RenderConfig)}", gameObject);
            }
            switch (_blinkType)
            {
                case BlinkType.SwitchMaterial:
                    if (_originalMaterial == null)
                    {
                        errorCount++;
                        Logger.LogError($"Original material is not set on {nameof(RenderConfig)}", gameObject);
                    }

                    if (_blinkMaterial == null)
                    {
                        errorCount++;
                        Logger.LogError($"Blink material is not set on {nameof(RenderConfig)}", gameObject);
                    }
                    break;
                case BlinkType.SwitchProperty:
                    break;
            }
        }

        public void Initialize()
        {
            if (_blinkType == BlinkType.SwitchProperty) 
                _mpb ??= new MaterialPropertyBlock();
            if (_blinkType == BlinkType.SwitchMaterial) 
                SetupOriginalMaterial();
        }

        public void SetOriginal()
        {
            switch (_blinkType)
            {
                case BlinkType.SwitchMaterial:
                    SetupOriginalMaterial();
                    break;
                case BlinkType.SwitchProperty:
                    SetupOriginalProperty();
                    break;
            }
        }

        public void SetBlink()
        {
            switch (_blinkType)
            {
                case BlinkType.SwitchMaterial:
                    SetupBlinkMaterial();
                    break;
                case BlinkType.SwitchProperty:
                    SetupBlinkProperty();
                    break;
            }
        }

        private void SetupOriginalProperty()
        {
            foreach (var renderer in _renderers) 
                renderer.SetPropertyBlock(null);
        }

        private void SetupBlinkProperty()
        {
            foreach (var renderer in _renderers)
            {
                renderer.GetPropertyBlock(_mpb);
                if(_switchFloat)
                    _mpb.SetFloat(_propertyName, _blinkPropertyValue);
                if(_switchColor)
                    _mpb.SetColor(_propertyColorName, _propertyColorValue);
                renderer.SetPropertyBlock(_mpb);
            }
        }

        private void SetupOriginalMaterial()
        {
            foreach (var renderer in _renderers) 
                renderer.sharedMaterial = _originalMaterial;   
        }

        private void SetupBlinkMaterial()
        {
            foreach (var renderer in _renderers) 
                renderer.sharedMaterial = _blinkMaterial;
        }
    }
    
    [DisallowMultipleComponent]
    public class BlinkEffect : VisualEffect, IBuildElement
    {
        [SerializeField] private RenderGroup _group;
        [SerializeField, MinValue(0)] private float _blinkDuration = 0.5f;
        
        private Tween _blinkTween;

        public RenderGroup Group => _group;

        void IBuildElement.LogIfWrong(ref int errorCount)
        {
            foreach (var renderConfig in _group.Configs) 
                renderConfig.LogIfWrong(ref errorCount, gameObject);
        }

        private void Awake() => 
            _group.Initialize();

        private void OnDisable() => 
            CancelBlink();

        public override void Activate(UnitView unitView)
        {
            base.Activate(unitView);
            Blink();
        }

        public override void Deactivate(UnitView unitView)
        {
            base.Deactivate(unitView);
            CancelBlink();
        }

        private void Blink()
        {
            if (_blinkTween.isAlive) 
                _blinkTween.progress = 0;
            else
            {
                _group.SetBlink();
                _blinkTween = Tween.Delay(_blinkDuration, () => _group.SetOriginal());
            }
        }

        private void CancelBlink()
        {
            if (!_blinkTween.isAlive) return;
            _blinkTween.Stop();
            _group.SetOriginal();
        }
    }
}
