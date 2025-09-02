using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Audio.Type;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window.Type;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Model
{
    public sealed class WindowService : IWindowService, ITickable, IDisposable
    {
        private Dictionary<string, BaseWindow> _cached = new();
        private readonly Dictionary<string, BaseWindow> _instantiated = new();
        private readonly Dictionary<string, AsyncLazy<BaseWindow>> _inLoading = new();

        private readonly List<BaseWindow> _openedWindows = new();

        private RectTransform _overHudRoot;
        private RectTransform _hudRoot;
        private RectTransform _underHudRoot;
        private RectTransform _overUI;

        private bool _popupPoolIsActive = true;
        private float _popupPoolTimer;
        private const float PopupPoolDelay = 1.5f;

        private readonly List<(BaseWindow window, Action action)> _popupPool = new();
        private readonly List<(BaseWindow window, Action action)> _openedPopups = new();
        
        [Inject] private ContentService _contentService;
        [Inject] private IAudioService _audioService;
        [Inject] private IObjectResolver _resolver;
        [Inject] private UIFactory _uiFactory;

        private readonly Dictionary<IUnlockable, (BaseWindow window, Action action)> _blockedPopups = new();

        public event Action<BaseWindow> OnOpen;
        public event Action<BaseWindow> OnClose;
        public event Action<string> OnCreate;

        [Preserve]
        public WindowService()
        {
        }

        void IDisposable.Dispose()
        {
            foreach (var openedWindow in _openedWindows)
            {
                openedWindow.OnOpen -= OnWindowOpen;
                openedWindow.OnClose -= OnWindowClose;
            }
        }

        void IWindowService.Setup(RectTransform overHudRoot, RectTransform hudRoot, RectTransform underHudRoot,
            RectTransform overUI)
        {
            _overHudRoot = overHudRoot;
            _hudRoot = hudRoot;
            _underHudRoot = underHudRoot;
            _overUI = overUI;
        }

        public async UniTask WarmUpAsync() => 
            _cached = await _uiFactory.WarmUpWindowsAsync();

        bool IWindowService.IsOpen<T>()
        {
            var type = typeof(T);
            return _instantiated.TryGetValue(type.Name, out var window) && window.IsOpened;
        }

        public bool IsExist<T>() =>
            _instantiated.ContainsKey(typeof(T).Name);

        public async UniTask WarmUpAsync(IReadOnlyList<string> windows, bool isImportant) => 
            await UniTask.WhenAll(windows.Select(item => GetAsync(item, isImportant)));

        public async UniTask<T> GetAsync<T>(bool isImportant) where T : BaseWindow => 
            (T)await GetAsync(typeof(T).Name, isImportant);

        private async UniTask<BaseWindow> GetAsync(string windowType, bool isImportant)
        {
            if (!_inLoading.TryGetValue(windowType, out var lazyLoading))
            {
                lazyLoading = GetPrefabAsync(windowType, isImportant).ToAsyncLazy();
                _inLoading.Add(windowType, lazyLoading);
            }
            var window = await lazyLoading;
            _inLoading.Remove(windowType);
            return window;
        }
        
        private async UniTask<BaseWindow> GetPrefabAsync(string windowType, bool isImportant)
        {
            if (_instantiated.TryGetValue(windowType, out var window)) return window;
            
            if (!_cached.TryGetValue(windowType, out var windowPrefab))
            {
                var windowGameObject = await _uiFactory.LoadAsync(windowType, isImportant);
                windowPrefab = windowGameObject.GetComponent<BaseWindow>();
                _cached[windowType] = windowPrefab;
            }

            var root = windowPrefab.ParentType switch
            {
                WindowParentType.OverHud => _overHudRoot,
                WindowParentType.Hud => _hudRoot,
                WindowParentType.UnderHud => _underHudRoot,
                WindowParentType.OverUI => _overUI,
                _ => _overHudRoot
            };

            window = _resolver.Instantiate(windowPrefab, root);
            _resolver.InjectGameObject(window.gameObject);
            _instantiated.Add(windowType, window);
            window.OnOpen += OnWindowOpen;
            window.OnClose += OnWindowClose;
            OnCreate?.Invoke(windowType);
            window.Close();

            return window;
        }

        public async UniTask<T> OpenAsync<T>(Action<T> setup = null) where T : BaseWindow
        {
            var window = await GetAsync<T>(true);
            setup?.Invoke(window);
            BaseOpen(window);
            return window;
        }

        public async UniTask<BaseWindow> OpenAsync(string windowType)
        {
            var window = await GetAsync(windowType, true);
            BaseOpen(window);
            return window;
        }

        private void BaseOpen(BaseWindow window)
        {
            window.transform.SetAsLastSibling();
            window.Open();
        }

        public async UniTask PushPopupAsync<T>(Action<T> setup = null) where T : BaseWindow
        {
            var window = await GetAsync<T>(false);
            Action action = () => setup?.Invoke(window);
            var unlockable = _contentService.GetPopupWindow(window.Id);
            if (unlockable is { IsUnlocked: false })
            {
                _blockedPopups.Add(unlockable, (window, action));
                unlockable.OnUnlocked += OnContentUnlocked;
            }
            else
                PushPopup(window, action);
        }

        private void OnContentUnlocked(IUnlockable unlockable)
        {
            unlockable.OnUnlocked -= OnContentUnlocked;
            if (_blockedPopups.Remove(unlockable, out var tuple))
                PushPopup(tuple.window, tuple.action);
        }

        private void PushPopup(BaseWindow window, Action action)
        {
            _popupPool.Add((window, action));
            if (_popupPool.Count == 1 && _popupPoolIsActive)
                ResetPopupPoolTimer();
        }

        private void PopPopup()
        {
            var tuple = _popupPool[0];
            _popupPool.RemoveAt(0);
            _openedPopups.Add(tuple);
            BaseOpen(tuple.window);
            tuple.action.Invoke();
        }

        public void BlockPopupPool()
        {
            _popupPoolIsActive = false;
            var openedPopupAmount = _openedPopups.Count;
            for (int i = 0; i < openedPopupAmount; i++)
            {
                var tuple = _openedPopups[0];
                _popupPool.Add(tuple);
                tuple.window.Close();
            }
        }

        public void UnlockPopupPool()
        {
            _popupPoolIsActive = true;
            ResetPopupPoolTimer();
        }

        public RectTransform GetOverUIRectTransform()
        {
            return _overUI;
        }

        private void ResetPopupPoolTimer() =>
            _popupPoolTimer = PopupPoolDelay;

        void IWindowService.Close<T>()
        {
            if (_instantiated.TryGetValue(typeof(T).Name, out var window))
                window.Close();
        }

        void IWindowService.Close(string windowType)
        {
            var window = GetOpenedWindowById(windowType);
            window?.Close();
        }

        void IWindowService.CloseAll(HashSet<string> exclusion)
        {
            for (var i = 0; i < _openedWindows.Count; i++)
            {
                var window = _openedWindows[i];
                var tuple = GetOpenedPopup(window);
                if (tuple != default) continue;
                if (exclusion == null || !exclusion.Contains(window.Id))
                {
                    _openedWindows.RemoveAt(i);
                    i--;
                    window.Close();
                }
            }
        }

        private (BaseWindow window, Action action) GetOpenedPopup(BaseWindow window)
        {
            foreach (var tuple in _openedPopups)
            {
                if (tuple.window == window)
                    return tuple;
            }

            return default;
        }

        public void Tick()
        {
            if (!_popupPoolIsActive || _popupPool.Count == 0 || _openedPopups.Count > 0) return;

            _popupPoolTimer -= Time.deltaTime;
            if (_popupPoolTimer > 0) return;
            PopPopup();
            ResetPopupPoolTimer();
        }

        private BaseWindow GetOpenedWindowById(string id)
        {
            foreach (var window in _openedWindows)
            {
                if (string.Equals(window.Id, id))
                    return window;
            }

            return null;
        }

        private void OnWindowOpen(BaseWindow window)
        {
            if (window.ParentType == WindowParentType.UnderHud) return;

            if (_openedWindows.Count == 0)
                _audioService.TransitTo(AudioSnapshot.Window);
            _openedWindows.Add(window);

            OnOpen?.Invoke(window);
        }

        private void OnWindowClose(BaseWindow window)
        {
            if (window.ParentType == WindowParentType.UnderHud) return;

            var tuple = GetOpenedPopup(window);
            if (tuple != default)
                _openedPopups.Remove(tuple);

            _openedWindows.Remove(window);
            if (_openedWindows.Count == 0)
                _audioService.TransitTo(AudioSnapshot.Normal);
            OnClose?.Invoke(window);
        }
    }
}