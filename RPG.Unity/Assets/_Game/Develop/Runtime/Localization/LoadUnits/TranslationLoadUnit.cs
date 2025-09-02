using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Localization.LoadUnits
{
    public class TranslationLoadUnit : ILoadUnit
    {
        public string DescriptionToken { get; }

        [Inject] private ITranslator _translator;

        [Preserve]
        public TranslationLoadUnit()
        {
        }
        
        public UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null)
        {
            progress?.Report(1);
            _translator.Initialize();
            return UniTask.CompletedTask;
        }
    }
}