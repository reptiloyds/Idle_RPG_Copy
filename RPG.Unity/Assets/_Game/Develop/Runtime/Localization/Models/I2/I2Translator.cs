using System;
using I2.Loc;
using PleasantlyGames.RPG.Runtime.Analytics.Contract;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.Utilities;
using VContainer;
using ILanguageSource = PleasantlyGames.RPG.Runtime.Localization.Contracts.ILanguageSource;

namespace PleasantlyGames.RPG.Runtime.Localization.Models.I2
{
    public sealed class I2Translator : ITranslator, IDisposable
    {
        [Inject] private ILanguageSource _languageSource;
        [Inject] private IAnalyticsService _analytics;
        
        public event Action OnChangeLanguage;

        [UnityEngine.Scripting.Preserve]
        public I2Translator() { }

        public void DetectLanguage() => 
            SetLanguage(_languageSource.GetLanguage());

        void ITranslator.Initialize()
        {
            _analytics.SendLanguage(_languageSource.GetLanguage());
            LocalizationManager.OnLocalizeEvent += OnLocalizeEvent;
        }

        void IDisposable.Dispose() => 
            LocalizationManager.OnLocalizeEvent -= OnLocalizeEvent;

        private void OnLocalizeEvent()
        {
            OnChangeLanguage?.Invoke();
        }

        private void SetLanguage(string language)
        {
            if(language.IsNullOrWhitespace()) return;
            
            LocalizationManager.SetLanguageAndCode(language, "");
        }

        public string Translate(string token)
        {
            var translation = LocalizationManager.GetTranslation(token);
            if (!string.IsNullOrEmpty(translation)) return translation;
            //Logger.LogWarning($"Can not find translation for token {token}");
            return token;

        }

        T ITranslator.LocalizeObject<T>(string token) =>
            LocalizationManager.GetTranslatedObjectByTermName<T>(token);
    }
}