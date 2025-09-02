using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.AssetManagement
{
    public interface ISpriteProvider
    {
        Sprite GetSprite(string spriteName);
        Sprite GetSprite(string atlasName, string spriteName);
        List<Sprite> GetSprites(string atlasName, List<string> spriteNames);
        UniTask WarmUpAsync();
        UniTask<Sprite> LoadSpriteAsync(string spriteKey, bool isImportant);
    }
}