using System;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.PeriodicRewards.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class PeriodicRewardView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _image;
        [SerializeField] private Image _background;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _value;
        [SerializeField] private TextMeshProUGUI _typeName;

        [SerializeField] private GameObject _completedObject;
        [SerializeField] private GameObject _currentObject;
        [SerializeField] private GameObject _completedAnimationObject;
        [SerializeField] private TweenSettings<Vector3> _completeAnimation;

        public RectTransform Rect => _rectTransform;
        
        public void Setup(Sprite sprite, Color color, string nameText, string valueText, string typeText)
        {
            _completedObject.SetActive(false);
            _currentObject.SetActive(false);

            _image.sprite = sprite;
            _background.color = color;
            _name.SetText(nameText);
            _value.SetText(valueText);
            _typeName.SetText(typeText);
        }

        public void AnimateCollection(Action callback = null)
        {
            MarkCompleted();
            var tween = Tween.Scale(_completedAnimationObject.transform, _completeAnimation);
            if (callback != null)
                tween.OnComplete(callback);
        }

        public void MarkCompleted() => 
            _completedObject.SetActive(true);

        public void MarkCurrent() => 
            _currentObject.SetActive(true);
    }
}