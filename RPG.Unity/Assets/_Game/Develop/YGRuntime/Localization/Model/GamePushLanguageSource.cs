using Cysharp.Threading.Tasks;
using GamePush;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.YGRuntime.Localization.Model
{
    public class GamePushLanguageSource : ILanguageSource
    {
        private GamePush.Language _language;

        private bool _languageIsIdentified;

        [Preserve]
        public GamePushLanguageSource() { }

        public UniTask IdentifyLanguage()
        {
            _language = GP_Language.Current();

            return UniTask.CompletedTask;
        }

        public string GetLanguage()
        {
            if (_language == GamePush.Language.English)
                return GamePush.Language.English.ToString();

            if (_language == GamePush.Language.Russian)
                return GamePush.Language.Russian.ToString();

            return GamePush.Language.English.ToString();
        }
    }
}