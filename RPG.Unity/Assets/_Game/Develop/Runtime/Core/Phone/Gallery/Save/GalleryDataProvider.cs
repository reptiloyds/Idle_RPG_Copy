using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Sheet;
using PleasantlyGames.RPG.Runtime.Save.Models;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Save
{
    [Serializable]
    public class GalleryDataContainer
    {
        public List<PhotoData> Photos = new();

        [Preserve]
        public GalleryDataContainer()
        {
        }
    }

    [Serializable]
    public class PhotoData
    {
        public string Key;
        public bool IsUnlocked;

        [Preserve]
        public PhotoData()
        {
        }
    }

    public class GalleryDataProvider : BaseDataProvider<GalleryDataContainer>
    {
        [Inject] private BalanceContainer _balance;

        [Preserve]
        public GalleryDataProvider()
        {
        }

        public override void LoadData()
        {
            base.LoadData();

            if (Data == null)
                CreateData();
            else
                ValidateData();
        }

        private void CreateData()
        {
            var sheet = _balance.Get<GallerySheet>();
            Data = new GalleryDataContainer();
            foreach (var config in sheet) 
                AddPhoto(config);
        }

        private void ValidateData()
        {
            var sheet = _balance.Get<GallerySheet>();
            foreach (var config in sheet)
            {
                if (!ContainKey(config.Id)) 
                    AddPhoto(config);
            }

            for (var i = 0; i < Data.Photos.Count; i++)
            {
                var photo = Data.Photos[i];
                if (sheet.Contains(photo.Key)) continue;
                Data.Photos.RemoveAt(i);
                i--;
            }
        }

        private void AddPhoto(GallerySheet.Row config)
        {
            Data.Photos.Add(new PhotoData()
            {
                Key = config.Id,
                IsUnlocked = false
            });
        }

        private bool ContainKey(string key)
        {
            foreach (var photo in Data.Photos)
            {
                if(string.Equals(photo.Key, key))
                    return true;
            }

            return false;
        }
    }
}