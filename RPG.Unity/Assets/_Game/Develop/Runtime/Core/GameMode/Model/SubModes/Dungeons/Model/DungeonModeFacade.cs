using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Ad.Contracts;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Contract;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Hub;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model
{
    public class DungeonModeFacade : IDisposable
    {
        [Inject] private IEnumerable<DungeonMode> _dungeons;
        [Inject] private IEnumerable<ILeveledDungeon> _leveledDungeons;
        [Inject] private IAdService _adService;
        [Inject] private ResourceService _resourceService;
        [Inject] private GameModeSwitcher _modeSwitcher;
        [Inject] private DungeonConfiguration _configuration;

        private DungeonMode _currentDungeonMode;
        private bool _isLaunched;

        public bool IsLaunched => _isLaunched;
        public IEnumerable<DungeonMode> Dungeons => _dungeons;
        public event Action<DungeonMode> OnLaunched;
        public event Action<DungeonMode> OnDisposed;

        [Preserve]
        public DungeonModeFacade()
        {
            
        }
        
        public void Initialize()
        {
            foreach (var dungeon in _dungeons)
            {
                dungeon.OnLaunched += OnDungeonLaunched;
                dungeon.OnDisposed += OnDungeonDisposed;
                dungeon.OnExitRequested += OnExitRequested;
            } 
        }

        void IDisposable.Dispose()
        {
            foreach (var dungeon in _dungeons)
            {
                dungeon.OnLaunched -= OnDungeonLaunched;
                dungeon.OnDisposed -= OnDungeonDisposed;
                dungeon.OnExitRequested -= OnExitRequested;
            } 
        }

        private void OnExitRequested(DungeonMode dungeonMode)
        {
            if (_currentDungeonMode != dungeonMode)
            {
                Debug.LogError("Request from other dungeon");
                return;
            }
            Exit();
        }

        private void OnDungeonLaunched(IGameMode mode)
        {
            if(_isLaunched) return;
            _isLaunched = true;
            OnLaunched?.Invoke(_currentDungeonMode);
        }

        private void OnDungeonDisposed(IGameMode mode)
        {
            if(!_isLaunched) return;
            _isLaunched = false;
            OnDisposed?.Invoke(_currentDungeonMode);
            _currentDungeonMode = null;
        }

        public ILeveledDungeon GetLeveledDungeon(GameModeType type)
        {
            foreach (var leveledDungeon in _leveledDungeons)
            {
                if (leveledDungeon.Type == type)
                    return leveledDungeon;
            }

            return null;
        }

        public void Launch(DungeonMode dungeonMode)
        {
            _currentDungeonMode = dungeonMode;
            foreach (var dung in _dungeons)
            {
                if(dung != dungeonMode) continue;
                _modeSwitcher.SwitchToAsync(dungeonMode.Type);
                break;
            }
        }

        public void BonusLaunch(DungeonMode dungeonMode)
        {
            _currentDungeonMode = dungeonMode;
            
            _currentDungeonMode.SpendBonusEnter();
            _currentDungeonMode.AddEnterResource();
            if (_modeSwitcher.ModeType == _currentDungeonMode.Type)
                Next(_currentDungeonMode);
            else
                _modeSwitcher.SwitchToAsync(_currentDungeonMode.Type);
        }

        public void Next(DungeonMode dungeonMode)
        {
            if(_currentDungeonMode != dungeonMode) return;
            if (dungeonMode.IsEnterResourceEnough)
            {
                _modeSwitcher.ExitFrom(dungeonMode.Type);
                Launch(dungeonMode);
            }
            else
            {
                _adService.OnRewardClosed += OnRestartAdClosed;
                _adService.ShowReward(AdId.LaunchDungeon);
            }
        }

        public void Exit() => 
            _modeSwitcher.SwitchToAsync(GameModeType.Main);

        private void OnRestartAdClosed(string id, bool success)
        {
            if(!string.Equals(AdId.LaunchDungeon, id)) return;
            _adService.OnRewardClosed -= OnRestartAdClosed;
            if (!success) return;

            _currentDungeonMode.SpendBonusEnter();
            var dungeon = _currentDungeonMode;
            _modeSwitcher.ExitFrom(_currentDungeonMode.Type);
            Launch(dungeon);
        }
    }
}