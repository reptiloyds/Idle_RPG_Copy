using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Save;
using PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Sheet;
using VContainer;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Model
{
    public class GalleryService
    {
        [Inject] private GalleryDataProvider _dataProvider;
        [Inject] private BalanceContainer _balance;
        [Inject] private ISpriteProvider _spriteProvider;

        private GalleryDataContainer _data;
        private readonly Dictionary<string, Photo> _photoMap = new();
        private readonly List<Photo> _photos = new();

        public IReadOnlyList<Photo> Photos => _photos;
        
        public void Initialize()
        {
            _data = _dataProvider.GetData();
            CreateModels();
        }
        
        public void UnlockPhoto(string key) => 
            GetPhoto(key)?.Unlock();

        private void CreateModels()
        {
            var sheet = _balance.Get<GallerySheet>();
            foreach (var data in _data.Photos)
            {
                if (!sheet.Contains(data.Key))
                {
                    Logger.LogError("Photo config not found");
                    return;
                }
                var config = sheet[data.Key];
                var photo = new Photo(config, data, _spriteProvider);
                _photoMap.Add(config.Id, photo);
                _photos.Add(photo);
            }
        }

        public async UniTask WarmUpLowQualityAsync(string key)
        {
            var photo = GetPhoto(key);
            if (photo != null) 
                await photo.GetLowQualitySpriteAsync();
        }

        public Photo GetPhoto(string key) => 
            _photoMap.GetValueOrDefault(key);
    }
}