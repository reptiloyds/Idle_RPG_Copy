using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Type;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model
{
    public class GameModeSwitcher : IDisposable
    {
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private IEnumerable<IGameMode> _gameModes;
        
        private GameModeType _modeType;
        private IGameMode _mode;

        public GameModeType ModeType => _modeType;
        
        [Preserve]
        public GameModeSwitcher()
        {
        }

        public void Initialize(GameModeType modeType)
        {
            _modeType = modeType;
            foreach (var gameMode in _gameModes) 
                gameMode.Initialize();
        }

        public void ExitFrom(GameModeType modeType)
        {
            if (_modeType != modeType)
            {
                Debug.LogError("Game mode is not " + modeType);
                return;
            }
            
            _mode.Dispose();
            _mode = null;
        }

        public async UniTask SwitchToAsync(GameModeType modeType)
        {
            if (_mode != null && _modeType == modeType)
            {
                Debug.LogError("Game mode already set to " + modeType);
                return;
            }

            IGameMode newMode = null;
            foreach (var gameMode in _gameModes)
            {
                if(gameMode.Type != modeType) continue;
                newMode = gameMode;
            }

            if (newMode == null)
            {
                Debug.LogError($"{typeof(IGameMode)} with type {modeType} not found");
                return;
            }

            _modeType = modeType;
            _mode?.Dispose();
            _mode = newMode;
            await _mode.LaunchAsync();
        }

        public void Dispose() => 
            _mode?.Dispose();
    }
}