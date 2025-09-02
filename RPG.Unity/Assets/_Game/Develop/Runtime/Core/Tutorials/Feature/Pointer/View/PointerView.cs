using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Pointer.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class PointerView : MonoBehaviour
    {
        [SerializeField, Required] private RectTransform _rectTransform;
        [SerializeField, Required] private RectTransform _view;
        [SerializeField, Required] private Image _maskImage;
        [SerializeField, Required] private Image _image;
        [SerializeField, Required] private Sprite _defaultSprite;
        [SerializeField, Required] private Sprite _flipSprite;

        private bool _isFollow;
        private RectTransform _followTarget;
        private Vector2 _offset;
        
        public RectTransform RectTransform => _rectTransform;

        // public void SetCustomSprite(Sprite sprite) => 
        //     _image.sprite = sprite == null ? _defaultSprite : sprite;

        public void ApplySettings(PointerSettings settings)
        {
            _isFollow = settings.Follow;
            if (_isFollow)
                _followTarget = settings.FollowTarget;

            RectTransform parent = null;
            switch (settings.ParentType)
            {
                case PointerParent.None:
                    break;
                case PointerParent.Target:
                    parent = settings.Target;
                    break;
                case PointerParent.Parent:
                    parent = settings.Target.parent.GetComponent<RectTransform>();
                    break;
                case PointerParent.ParentParent:
                    parent = settings.Target.parent.parent.GetComponent<RectTransform>();
                    break;
                // case PointerParent.UpperCanvas: 
                //     var canvases = settings.Target.GetComponentsInParent<Canvas>();
                //     int minOrder = int.MaxValue;
                //     Transform result = null;
                //     foreach (var canvas in canvases)
                //     {
                //         if (canvas.sortingOrder > minOrder) continue;
                //         minOrder = canvas.sortingOrder;
                //     }
                //
                //     parent = result!.GetComponent<RectTransform>();
                //     break;
            }

            _offset = settings.Offset;
            if (parent != null)
            {
                _rectTransform.SetParent(parent);
                if (settings.OffsetPosition) 
                    _rectTransform.anchoredPosition = _offset;
                else
                    _rectTransform.anchoredPosition = Vector2.zero;   
            }
            else
            {
                _rectTransform.position = settings.Target.position;
                _rectTransform.anchoredPosition += _offset;
            }

            if (settings.Flip)
            {
                _maskImage.sprite = _flipSprite;
                _image.sprite = _flipSprite;
            }
            else
            {
                _maskImage.sprite = _defaultSprite;
                _image.sprite = _defaultSprite;
            }

            if(settings.ChangeRotation)
                _view.localEulerAngles = settings.Rotation;
            else
                _view.localEulerAngles = Vector3.zero;
        }

        private void LateUpdate()
        {
            if(!_isFollow) return;
            _rectTransform.position = _followTarget.position;
            _rectTransform.anchoredPosition += _offset;
        }

        private void OnDisable()
        {
            _isFollow = false;
            _followTarget = null;
        }
    }
}