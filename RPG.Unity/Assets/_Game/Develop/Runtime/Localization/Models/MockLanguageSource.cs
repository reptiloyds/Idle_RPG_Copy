using Cathei.BakingSheet.Internal;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Localization.Models
{
    public class MockLanguageSource : ILanguageSource
    {
        [Preserve]
        public MockLanguageSource()
        {
        }
        
        public string GetLanguage() => 
            Application.systemLanguage.ToString();
    }
}