using System;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Save;
using PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Sheet;
using R3;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Model
{
    public class Photo : IDisposable
    {
        private readonly GallerySheet.Row _config;
        private readonly PhotoData _data;
        private readonly ISpriteProvider _spriteProvider;
        private readonly CompositeDisposable _compositeDisposable = new();

        private Sprite _lowQualitySprite;
        private Sprite _highQualitySprite;
        
        private readonly string _lowQualityKey; 
        private readonly string _highQualityKey;
        private readonly ReactiveProperty<bool> _isUnlocked;
        
        public bool HasLowQualityPhoto => _lowQualitySprite != null;
        public bool HasHighQualityPhoto => _highQualitySprite != null;

        public static string LowQualityPostfix { get; private set; } = "_L";
        public static string HighQualityPostfix { get; private set; } = "_H";
        
        public ReadOnlyReactiveProperty<bool> IsUnlocked => _isUnlocked;

        public Photo(GallerySheet.Row config, PhotoData data, ISpriteProvider spriteProvider)
        {
            _config = config;
            _data = data;
            _spriteProvider = spriteProvider;

            _lowQualityKey = _config.SpriteName + LowQualityPostfix;
            _highQualityKey = _config.SpriteName + HighQualityPostfix;
            _isUnlocked = new ReactiveProperty<bool>(_data.IsUnlocked);
            _isUnlocked.Subscribe(value => _data.IsUnlocked = value)
                .AddTo(_compositeDisposable);
        }

        public void Unlock() => 
            _isUnlocked.Value = true;

        public async UniTask<Sprite> GetLowQualitySpriteAsync()
        {
            if (_lowQualitySprite != null) return _lowQualitySprite;
            _lowQualitySprite = await _spriteProvider.LoadSpriteAsync(_lowQualityKey, false);
            return _lowQualitySprite;
        }
        
        public async UniTask<Sprite> GetHighQualitySpriteAsync()
        {
            if (_highQualitySprite != null) return _highQualitySprite;
            _highQualitySprite = await _spriteProvider.LoadSpriteAsync(_highQualityKey, false);
            return _highQualitySprite;
        }

        public void Dispose() => 
            _compositeDisposable?.Dispose();
    }
}