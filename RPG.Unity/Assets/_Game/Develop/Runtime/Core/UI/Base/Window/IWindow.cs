using System;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Base.Window
{
    public interface IWindow
    {
        string Id { get; }
        GameObject GameObject { get; }
        public event Action<BaseWindow> OnOpen;
        public event Action<BaseWindow> OnClose;
        
        void Open();
        void Close();
    }
}