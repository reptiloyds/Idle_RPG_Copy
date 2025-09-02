using System;
using Object = UnityEngine.Object;

namespace PleasantlyGames.RPG.Runtime.Localization.Contracts
{
    public interface ITranslator
    {
        event Action OnChangeLanguage;

        void DetectLanguage();
        void Initialize();
        string Translate(string token);
        T LocalizeObject<T>(string token) where T : Object;
    }
}