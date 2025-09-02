using System;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Condition;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Condition.MainMode.Complete;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Condition.MainMode.Enter;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Condition.Quest;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Save;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Sheet;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Type;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.ContentControl.Model
{
    public class Content : IUnlockable, IDisposable
    {
        [Inject] private MainMode _mainMode;
        [Inject] private QuestService _questService;

        private readonly ITranslator _translator;
        private readonly ISpriteProvider _spriteProvider;

        private readonly ContentData _data;
        private readonly ContentSheet.Row _config;
        private UnlockCondition _condition;
        private IUnlockable _dependency;
        private bool IsDependencyUnlocked => _dependency == null || _dependency.IsUnlocked;

        public string Id => _data.Id;
        public string Name { get; private set; }
        public Sprite Image { get; private set; }
        public string Dependency => _config.Dependency;
        public ContentType Type => _config.ContentType;

        public bool IsManualUnlock => _config.ManualUnlock;
        public bool IsUnlocked => _data.IsUnlocked;
        public string Condition => _condition.GetDescription();
        public float Progress => _condition.Progress;
        public bool IsReadyForManualUnlock { get; private set; }

        public event Action<IUnlockable> OnUnlocked;
        public event Action OnProgressChanged;
        public event Action OnReadyToManualUnlock;

        public Content(ContentData data, ContentSheet.Row config,
            ITranslator translator, ISpriteProvider spriteProvider)
        {
            _data = data;
            _config = config;

            _translator = translator;
            _spriteProvider = spriteProvider;

            if (!string.IsNullOrEmpty(config.LocalizationToken))
                Name = _translator.Translate(config.LocalizationToken);
            else
                Name = string.Empty;

            if (!string.IsNullOrEmpty(config.ImageName))
                Image = spriteProvider.GetSprite(config.ImageName);
        }

        public void SetupDependency(Content dependency)
        {
            _dependency = dependency;
            if (_dependency is { IsUnlocked: false })
                _dependency.OnUnlocked += OnDependencyUnlocked;
        }

        public void Initialize()
        {
            if (_data.IsUnlocked) return;

            switch (_config.UnlockType)
            {
                case UnlockType.MainModeEnter:
                    _condition = new MainModeEnterCondition(_config, _translator, _mainMode);
                    break;
                case UnlockType.MainModeComplete:
                    _condition = new MainModeCompleteCondition(_config, _translator, _mainMode);
                    break;
                case UnlockType.QuestCollect:
                    _condition = new QuestCollectCondition(_config, _translator, _questService);
                    break;
            }

            _condition.OnCompleted += OnCompleted;
            _condition.OnProgressChanged += OnConditionProgressChanged;
            _condition.Initialize();
        }

        private void OnDependencyUnlocked(IUnlockable content)
        {
            content.OnUnlocked -= OnDependencyUnlocked;
            CheckState();
        }

        private void OnCompleted() =>
            CheckState();

        private void OnConditionProgressChanged() =>
            OnProgressChanged?.Invoke();

        private void CheckState()
        {
            if (!IsDependencyUnlocked) return;
            if (!_condition.IsCompleted) return;
            if (!IsManualUnlock)
                Unlock();
            else if(!IsReadyForManualUnlock)
            {
                IsReadyForManualUnlock = true;
                OnReadyToManualUnlock?.Invoke();
            }
        }

        public void ManualUnlock()
        {
            if(!IsReadyForManualUnlock) return;
            Unlock();
        }

        internal void Unlock()
        {
            _data.IsUnlocked = true;
            OnUnlocked?.Invoke(this);
        }

        public void Dispose()
        {
            if (_condition != null)
            {
                _condition.OnCompleted -= OnCompleted;
                _condition.OnProgressChanged -= OnConditionProgressChanged;
            }

            if (_dependency is { IsUnlocked: false })
                _dependency.OnUnlocked -= OnDependencyUnlocked;
        }
    }
}