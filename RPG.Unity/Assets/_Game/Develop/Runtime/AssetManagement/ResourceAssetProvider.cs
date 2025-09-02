using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.AssetManagement
{
    public sealed class ResourceAssetProvider
    {
        public GameObject LoadAsset(string path)
        {
            var asset = Resources.Load<GameObject>(path);
            if (asset == null) 
                Debug.LogError($"Can`t find resource type of {typeof(GameObject)} by path {path}");
            return asset;
        }

        public T LoadAsset<T>(string path) where T : Object
        {
            var asset = Resources.Load<T>(path);
            if (asset == null) 
                Debug.LogError($"Can`t find resource type of {typeof(T)} by path {path}");
            return asset;
        }

        public T[] LoadAllAssets<T>(string path) where T : Object
        {
            var assets = Resources.LoadAll<T>(path);
            if (assets == null || assets.Length == 0) 
                Debug.LogError($"Can`t find resources type of {typeof(T)} by path {path}");
            return assets;
        }
    }
}