using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Audio.Type;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Settings.Model
{
    public class SettingsService : IInitializable
    {
        private readonly IAudioService _audioService;
        private readonly ProductService _productService;

        private const string MusicKey = "Settings_Music";
        private const string EffectsSoundsKey = "Settings_EffectsSounds";

        public bool IsMusicEnabled = true;
        public bool IsEffectsSoundsEnabled = true;

        [Inject] [Preserve]
        public SettingsService(IAudioService audioService, ProductService productService)
        {
            _audioService = audioService;
            _productService = productService;
        }

        public void Initialize()
        {
            if (PlayerPrefs.HasKey(MusicKey))
                IsMusicEnabled = PlayerPrefs.GetFloat(MusicKey) > 0;
            _audioService.SetVolume(AudioGroup.Music, IsMusicEnabled ? 1 : 0);

            if (PlayerPrefs.HasKey(EffectsSoundsKey)) 
                IsEffectsSoundsEnabled = PlayerPrefs.GetFloat(EffectsSoundsKey) > 0;
            _audioService.SetVolume(AudioGroup.SFX, IsEffectsSoundsEnabled ? 1 : 0);
        }

        public void ToggleMusic()
        {
            IsMusicEnabled = !IsMusicEnabled;
            var value = IsMusicEnabled ? 1 : 0;
            PlayerPrefs.SetFloat(MusicKey, value);
            _audioService.SetVolume(AudioGroup.Music, value);
        }

        public void ToggleEffectsSounds()
        {
            IsEffectsSoundsEnabled = !IsEffectsSoundsEnabled;
            var value = IsEffectsSoundsEnabled ? 1 : 0;
            PlayerPrefs.SetFloat(EffectsSoundsKey,  value);
            _audioService.SetVolume(AudioGroup.SFX, value);
        }

        public bool NeedRestorePurchasesManual()
        {
#if UNITY_IOS
            return true;
#endif
            return false;
        }

        public void RestorePurchases()
        {
            
        }
    }
}