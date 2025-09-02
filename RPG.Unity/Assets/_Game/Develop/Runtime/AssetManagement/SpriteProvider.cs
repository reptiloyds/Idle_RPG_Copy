using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;
using VContainer;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

namespace PleasantlyGames.RPG.Runtime.AssetManagement
{
    public class SpriteProvider : ISpriteProvider
    {
        private readonly Dictionary<string, SpriteAtlas> _atlasMap = new();
        private readonly Dictionary<string, Sprite> _spriteMap = new();
        
        private readonly Dictionary<string, AsyncLazy<Sprite>> _inLoading = new();

        private readonly IAssetProvider _assetProvider;

        [Preserve]
        [Inject]
        public SpriteProvider(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public async UniTask WarmUpAsync()
        {
            var atlases = WarmUpAtlases();
            var singleSprites = WarmUpSingleSprites();
            await UniTask.WhenAll(atlases, singleSprites);
        }

        private async UniTask WarmUpAtlases()
        {
            var locations = await _assetProvider.GetResourceLocationsAsync(AssetLabel.Atlas, Addressables.MergeMode.Union,
                typeof(SpriteAtlas));
            var results = await _assetProvider.WarmUp<SpriteAtlas>(locations);
            for (var i = 0; i < locations.Count; i++)
                _atlasMap[locations[i].PrimaryKey] = results[i];
        }

        private async UniTask WarmUpSingleSprites()
        {
            var locations = await _assetProvider.GetResourceLocationsAsync(AssetLabel.SingleSprite,
                Addressables.MergeMode.Union, typeof(Sprite));
            var results = await _assetProvider.WarmUp<Sprite>(locations);
            for (var i = 0; i < locations.Count; i++)
                _spriteMap[locations[i].PrimaryKey] = results[i];
        }

        public async UniTask<Sprite> LoadSpriteAsync(string spriteKey, bool isImportant)
        {
            if (!_inLoading.TryGetValue(spriteKey, out var lazyLoading))
            {
                lazyLoading = GetSpriteAsync(spriteKey, isImportant).ToAsyncLazy();
                _inLoading.Add(spriteKey, lazyLoading);
            }
            var sprite = await lazyLoading;
            _inLoading.Remove(spriteKey);
            return sprite;
        }
        
        private async UniTask<Sprite> GetSpriteAsync(string spriteKey, bool isImportant)
        {
            if (_spriteMap.TryGetValue(spriteKey, out var sprite)) return sprite;
            sprite = await _assetProvider.LoadAssetAsync<Sprite>(spriteKey, isImportant);
            _spriteMap.Add(spriteKey, sprite);
            return sprite;
        }

        public Sprite GetSprite(string spriteName)
        {
            if (spriteName == null) return null;
            var sprite = _spriteMap.GetValueOrDefault(spriteName);
            if (sprite == null)
                sprite = SearchInAtlases(spriteName);

            return sprite;
        }
        
        public Sprite GetSprite(string atlasName, string spriteName)
        {
            if (!_atlasMap.TryGetValue(atlasName, out var atlas)) return null;
            var sprite = atlas.GetSprite(spriteName);
            return sprite != null ? sprite : GetSprite(spriteName);
        }

        public List<Sprite> GetSprites(string atlasName, List<string> spriteNames)
        {
            var sprites = new List<Sprite>(spriteNames.Count);
            foreach (var spriteName in spriteNames)
                sprites.Add(GetSprite(atlasName, spriteName));

            return sprites;
        }

        private Sprite SearchInAtlases(string spriteName)
        {
            Sprite result = null;
            foreach (var kvp in _atlasMap)
            {
                result = kvp.Value.GetSprite(spriteName);
                if (result != null)
                    return result;
            }

            return result;
        }

        public static bool HasSprite(string atlasName, string spriteName)
        {
#if !UNITY_EDITOR
            return false;
#elif UNITY_EDITOR
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("Can`t find Addressable settings");
                return false;
            }

            foreach (AddressableAssetGroup group in settings.groups)
            foreach (AddressableAssetEntry entry in group.entries)
                if (entry.address == atlasName)
                {
                    SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(entry.AssetPath);
                    var sprite = atlas.GetSprite(spriteName);

                    if (sprite == null)
                    {
                        Debug.LogWarning($"Can`t find sprite by name {spriteName} in atlas {atlasName}");
                        return false;
                    }

                    return true;
                }

            Debug.Log($"SpriteAtlas by key {atlasName} does not exists in Addressable");
            return false;
#endif
        }
    }
}