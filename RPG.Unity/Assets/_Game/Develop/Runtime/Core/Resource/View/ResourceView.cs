using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.TooltipsFeature.Factory;
using PleasantlyGames.RPG.Runtime.UIImpact;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Resource.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class ResourceView : MonoBehaviour
    {
        [SerializeField, Required] private Image _icon;
        [SerializeField, Required] private TextMeshProUGUI _text;
        [SerializeField, Required] private UIImpactListener _impactListener;
        [SerializeField] private bool _formatBigValue = true;

        private bool _invisibleWhenZero;

        private ResourceModel _resourceModel;
        private BigDouble.Runtime.BigDouble _preventValue;
        private BigDouble.Runtime.BigDouble _redrawDelta;

        public ResourceType Type => _resourceModel.Type;

        public void Setup(ResourceModel resourceModel, ResourceViewSettings settings = null)
        {
            _resourceModel = resourceModel;
            _resourceModel.OnChange += ResourceModelChange;
            _preventValue = _resourceModel.Value;

            _icon.sprite = resourceModel.Sprite;
            _invisibleWhenZero = resourceModel.InvisibleWhenZero;
            if (settings != null) 
                _formatBigValue = settings.FormatBigValue;
            
            Redraw();
        }

        public void Enable() => gameObject.SetActive(true);
        public void Disable() => gameObject.SetActive(false);

        public void AddRedrawDelta(BigDouble.Runtime.BigDouble redrawDelta)
        {
            _redrawDelta += redrawDelta;
        }

        public void RemoveRedrawDelta(BigDouble.Runtime.BigDouble redrawDelta)
        {
            _redrawDelta -= redrawDelta;
            if (BigDouble.Runtime.BigDouble.Abs(_redrawDelta) < double.Epsilon)
                _redrawDelta = 0;
        }
        
        private void ResourceModelChange() => Redraw();

        public void Redraw()
        {
            if (_invisibleWhenZero) 
                CheckAmount();

            var result = _resourceModel.Value + _redrawDelta;
            if (result < 0)
                result = 0;
            
            if (result > _preventValue) 
                _impactListener.Play();

            if (_formatBigValue)
                _text.SetText(StringExtension.Instance.CutBigDouble(result, true));
            else
                _text.SetText(StringExtension.Instance.RoundBigDouble(result));
            _preventValue = result;
        }

        private void CheckAmount()
        {
            if (_resourceModel.Value == 0)
                gameObject.Off();
            else
                gameObject.On();
        }

        private void OnDestroy()
        {
            _resourceModel.OnChange -= ResourceModelChange;
            _resourceModel = null;
        }
    }
}
