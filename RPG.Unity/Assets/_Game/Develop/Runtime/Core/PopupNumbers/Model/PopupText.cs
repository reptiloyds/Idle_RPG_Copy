using System;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.PopupNumbers.Model
{
    [DisallowMultipleComponent, HideMonoScript]
    public class PopupText : MonoBehaviour
    {
        [SerializeField] private RectTransform _rect;
        [SerializeField, Required] private TextMeshProUGUI _content;
        [SerializeField, MinValue(0)] private float _moveDuration = 0.5f;
        [SerializeField, Required] private Transform _moveTarget;
        [SerializeField] private Ease _moveEase = Ease.OutSine;
        [SerializeField] private TweenSettings<float> _hideSettings;

        [SerializeField, Required] private Transform _spawnLeftDownAnchor;
        [SerializeField, Required] private Transform _spawnRightUpAnchor;

        public RectTransform Rect => _rect;
        public event Action<PopupText> OnDisappear; 

        public void Show(string content, Color color)
        {
            _content.SetText(content);
            _content.color = color;
            _content.alpha = 1;
        }

        public Vector3 GetOffset()
        {
            return new Vector3(new Vector2(_spawnLeftDownAnchor.localPosition.x, _spawnRightUpAnchor.localPosition.x).Random(),
                new Vector2(_spawnLeftDownAnchor.localPosition.y, _spawnRightUpAnchor.localPosition.y).Random());
        }

        public void PlayAnimation()
        {
            var sequence = Sequence.Create();
            sequence.Chain(Tween.Position(transform, _moveTarget.position, _moveDuration, _moveEase));
            sequence.Chain(Tween.Alpha(_content, _hideSettings));
            sequence.OnComplete(Disappear);
        }

        private void Disappear() => 
            OnDisappear?.Invoke(this);
    }
}