using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract
{
    public interface IWindowService
    {
        void Setup(RectTransform overHudRoot, RectTransform hudRoot, RectTransform underHudRoot, RectTransform overUI);
        event Action<BaseWindow> OnOpen;
        event Action<BaseWindow> OnClose;
        event Action<string> OnCreate;

        UniTask WarmUpAsync();
        bool IsOpen<T>();
        bool IsExist<T>();
        UniTask WarmUpAsync(IReadOnlyList<string> windows, bool isImportant);
        UniTask<T> GetAsync<T>(bool isImportant) where T : BaseWindow;
        UniTask<T> OpenAsync<T>(Action<T> setup = null) where T : BaseWindow;
        UniTask<BaseWindow> OpenAsync(string windowType);
        void Close<T>() where T : BaseWindow;
        void Close(string windowType);
        void CloseAll(HashSet<string> exclusion = null);
        UniTask PushPopupAsync<T>(Action<T> setup = null) where T : BaseWindow;
        void BlockPopupPool();
        void UnlockPopupPool();
        RectTransform GetOverUIRectTransform();
    }
}