using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Tween;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class BlessingPreview : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Image _upArrow;
        [SerializeField] private Image _downArrow;
        [SerializeField] private Color _upArrowColor;
        [SerializeField] private Color _downArrowColor;
        [SerializeField] private Color _disableGraphicColor;
        [SerializeField] private Image _background;
        [SerializeField] private Color _disabledBackColor;
        [SerializeField] private Color _enabledBackColor;
        [SerializeField] private UIBlink _blink;
        
        private Blessing _blessing;

        private void OnDestroy() => 
            ClearBlessing();

        private void ClearBlessing()
        {
            if (_blessing == null) return;
            _blessing.OnEnabled -= OnBlessingEnabled;
            _blessing.OnDisabled -= OnBlessingDisabled;
            _blessing = null;
        }

        public void Setup(Blessing blessing)
        {
            ClearBlessing();
            
            _blessing = blessing;
            _image.sprite = _blessing.Sprite;
            _upArrow.gameObject.SetActive(_blessing.IsIncreaseEffect);
            _downArrow.gameObject.SetActive(!_blessing.IsIncreaseEffect);

            _blessing.OnEnabled += OnBlessingEnabled;
            _blessing.OnDisabled += OnBlessingDisabled;

            if (_blessing.IsActive.CurrentValue)
                Tween.Delay(0.1f, SetEnabledVisual);
            else
                SetDisabledVisual();
        }

        private void OnBlessingEnabled(Blessing blessing) => 
            SetEnabledVisual();
        
        private void OnBlessingDisabled(Blessing blessing) => 
            SetDisabledVisual();

        private void SetEnabledVisual()
        {
            _background.color = _enabledBackColor;
            _image.color = Color.white;
            _upArrow.color = _upArrowColor;
            _downArrow.color = _downArrowColor;
            _blink.Play();
        }

        private void SetDisabledVisual()
        {
            _background.color = _disabledBackColor;
            _image.color = _disableGraphicColor;
            _upArrow.color = _disableGraphicColor;
            _downArrow.color = _disableGraphicColor;
            _blink.Stop();
        }
    }
}
