using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Balance.Contract;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Type;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.Quests.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Model;
using PleasantlyGames.RPG.Runtime.InAppPurchase.Contract;
using PleasantlyGames.RPG.Runtime.Save.Contracts;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Cheats
{
    public class CheatService : IInitializable, IDisposable, ITickable
    {
        [Inject] private ISaveService _saveService;
        [Inject] private ResourceService _resourceService;
        [Inject] private MainMode _mainMode;
        [Inject] private StatImprover _statImprover;
        [Inject] private QuestService _questService;
        [Inject] private TutorialService _tutorialService;
        [Inject] private IInAppProvider _inAppPurchase;
        [Inject] private GameModeSwitcher _switcher;
        [Inject] private ContentService _contentService;
        [Inject] private IBalanceProvider _balanceProvider;
        [Inject] private UnitStatService _statService;
        [Inject] private ItemFacade _itemFacade;

        [Inject] private CheatConfiguration _config;
        [Inject] private TimeService _time;

        private bool _isInputEnabled = true;
        private UnityEngine.Camera _camera;

        public bool IsEnabled { get; }

        public event Action OnBalanceFileChanged;

        public CheatService()
        {
#if RPG_DEV && !CHEATS_DISABLED
            IsEnabled = true;
#else
            IsEnabled = false;
#endif
        }

        public void Initialize() => 
            _balanceProvider.OnFileNameChanged += OnBalanceFileNameChanged;

        public void Dispose() => 
            _balanceProvider.OnFileNameChanged -= OnBalanceFileNameChanged;

        private void OnBalanceFileNameChanged() => 
            OnBalanceFileChanged?.Invoke();

        public void EnableInput() => 
            _isInputEnabled = true;

        public void DisableInput() => 
            _isInputEnabled = false;

        public void Tick()
        {
#if UNITY_EDITOR
            if(!_isInputEnabled) return;
            
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
                SetTimeScale(1);
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
                SetTimeScale(2);
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
                SetTimeScale(3);
            else if (Keyboard.current.digit4Key.wasPressedThisFrame)
                SetTimeScale(4);
            else if (Keyboard.current.sKey.wasPressedThisFrame)
                Save();
            else if (Keyboard.current.qKey.wasPressedThisFrame) 
                SetResource(ResourceType.Soft, 1000000);
#endif
        }

        public void SetTimeScale(float timeScale) => 
            Time.timeScale = timeScale;

        public void Save() => 
            _saveService.SaveAndLoadToCloudAsync();

        public void SetResource(ResourceType type, BigDouble.Runtime.BigDouble amount)
        {
            var resource = _resourceService.GetResource(type);
            resource.Set(amount);
        }

        public void ChangeLevel(int period, int level) => 
            _mainMode.CheatSwitch(period, level);

        public List<UnitStatType> GetAllStatTypes()
        {
            var statTypes = new List<UnitStatType>();
            foreach (var stat in _statImprover.Stats) 
                statTypes.Add(stat.Type);

            return statTypes;
        }

        public void SetParamLevel(UnitStatType statType, int level)
        {
            if(level < 1) return;
            var stat = _statImprover.GetStat(statType);
            stat.SetLevel(level - 1);
            stat.LevelUp();
        }

        public void WarpQuest(int id) => 
            _questService.WarpTo(id);

        public void TestTutorial(string id) =>
            _tutorialService.TestTutorialAsync(id);

        public void TestInternalLogic() => 
            _time.Test_IncrementDays(1);

        public void UnlockContent()
        {
            _contentService.Unlock(_config.UnlockContentTypes);
            _statImprover.UnlockAllCondition();
            var stats = _statService.GetPlayerStats();
            foreach (var stat in stats)
            {
                if (!stat.IsUnlocked) 
                    stat.Unlock();
            }
        }

        public void UnlockBranches() => 
            _contentService.Unlock(ContentType.Branch);

        public void UnlockItems()
        {
            foreach (var item in _itemFacade.Items) 
                _itemFacade.AddAmount(item.Id, 1);
        }

        public void AppendItem(string id) => 
            _itemFacade.AddAmount(id, 1);

        public string BalanceFileName =>
            _balanceProvider.FileName;
    }
}