using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Base.Window.Animation
{
    [HideMonoScript]
    public abstract class BaseWindowAnimation : MonoBehaviour
    {
        [Flags]
        protected enum WindowAnimationType
        {
            Open = 1 << 0,
            Close = 1 << 1,
        }
        
        [SerializeField] protected WindowAnimationType _animationType;
        
        public abstract float GetOpenDuration();
        public abstract float GetCloseDuration();
        
        public abstract void Open();
        public abstract void Close();
    }
}