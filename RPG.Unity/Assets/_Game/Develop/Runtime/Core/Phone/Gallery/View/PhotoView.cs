using System;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class PhotoView : MonoBehaviour
    {
        [SerializeField] private RectTransform _selftRect;
        [SerializeField] private Image _image;
        [SerializeField] private GameObject _placeholder;
        [SerializeField] private GameObject _loading;
        [SerializeField] private BaseButton _button;
        [SerializeField] private bool _highQuality;
        [SerializeField] private bool _useDelayBeforLoading;
        [SerializeField] private bool _openViewerByClick;

        [Inject] private IWindowService _windowService;

        private readonly SerialDisposable _serialDisposable = new();
        private Photo _photo;

        public RectTransform SelfRect => _selftRect;
        public Photo Photo => _photo;
        
        private void Awake()
        {
            _button.OnClick += OnButtonClick;
            _placeholder.SetActive(true);
            _loading.SetActive(false);
        }

        private void OnDestroy()
        {
            _button.OnClick -= OnButtonClick;
            _serialDisposable.Dispose();
        }

        public void Setup(Photo photo)
        {
            _photo = photo;
            _serialDisposable.Disposable = _photo.IsUnlocked.Subscribe(_ => UpdateState());
        }

        public void Clear()
        {
            _photo = null;
            _serialDisposable.Disposable?.Dispose();
        }

        private async void UpdateState()
        {
            if (_photo.IsUnlocked.CurrentValue)
            {
                _placeholder.SetActive(false);
                _loading.SetActive(true);
                _image.gameObject.SetActive(false);

                var hasPhoto = _highQuality ? _photo.HasHighQualityPhoto : _photo.HasLowQualityPhoto;
                if (!hasPhoto && _useDelayBeforLoading)
                {
                    await UniTask.DelayFrame(3);
                    if(!gameObject.activeSelf) return;   
                }
                
                var sprite = await (_highQuality
                    ? _photo.GetHighQualitySpriteAsync()
                    : _photo.GetLowQualitySpriteAsync());
                
                if(!gameObject.activeSelf) return;
                _image.sprite = sprite;
                _loading.SetActive(false);
                _image.gameObject.SetActive(true);
            }
            else
            {
                _placeholder.SetActive(true);
                _loading.SetActive(false);
                _image.gameObject.SetActive(false);
            }
        }

        private async void OnButtonClick()
        {
            if (!_openViewerByClick) return;
            if(_photo == null) return;
            if(!_photo.IsUnlocked.CurrentValue) return;
            var window = await _windowService.OpenAsync<PhotoViewerWindow>();
            window.Setup(Photo);
        }
    }
}