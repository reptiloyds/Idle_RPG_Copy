using System;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Sheet;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;

namespace PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Condition
{
    public abstract class UnlockCondition
    {
        private readonly ContentSheet.Row _config;
        protected readonly ITranslator Translator;
        protected string LocalizationKey => $"{_config.UnlockType}{TranslationConst.ConditionLocalizationPostfix}";

        public bool IsCompleted { get; private set; }

        private float _progress;

        public float Progress
        {
            get => _progress;
            protected set
            {
                _progress = value;
                OnProgressChanged?.Invoke();
            }
        }

        public event Action OnProgressChanged;
        public event Action OnCompleted;

        protected UnlockCondition(ContentSheet.Row config, ITranslator translator)
        {
            _config = config;
            Translator = translator;
        }

        public virtual void Initialize()
        {
            CreateData(_config.UnlockDataJSON);
        }

        protected abstract void CreateData(string dataJson);

        protected virtual void Complete()
        {
            Clear();
            IsCompleted = true;
            OnCompleted?.Invoke();
        }

        protected virtual void Clear()
        {
            
        }
        
        public abstract string GetDescription();
    }
}