using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window.Type;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Base.Window
{
    [HideMonoScript, DisallowMultipleComponent]
    public abstract class BaseWindow : MonoBehaviour
    {
        [SerializeField] private string _id;
        [SerializeField, HideIn(PrefabKind.InstanceInScene)] private WindowParentType _parentType;
        [SerializeField] protected bool UseUnscaledTime = true;
        [SerializeField] private List<BaseButton> _closeButtons;
        [SerializeField] private bool _blockInteractionWhileAnimated = true;

        [SerializeField, HideIf("@this._blockInteractionWhileAnimated == false")]
        protected CanvasGroup _canvasGroup;

        private Action _completeOpening;
        private Action _completeClosing;

        public bool IsOpened { get; private set; }

        public string Id => _id;
        public WindowParentType ParentType => _parentType;

        #region UnityEvents

        [SerializeField, FoldoutGroup("UnityEvents")] private UnityEvent OnOpenUnityEvent;
        [SerializeField, FoldoutGroup("UnityEvents")] private UnityEvent OnCloseUnityEvent;
        [SerializeField, FoldoutGroup("UnityEvents")] private UnityEvent OnOpenedUnityEvent;
        [SerializeField, FoldoutGroup("UnityEvents")] private UnityEvent OnClosedUnityEvent;

        #endregion

        public event Action<BaseWindow> OnOpen;
        public event Action<BaseWindow> OnClose;

        public event Action<BaseWindow> OnOpened;
        public event Action<BaseWindow> OnClosed;

        protected virtual void Awake()
        {
            _completeOpening = CompleteOpening;
            _completeClosing = CompleteClosing;
            foreach (var closeButton in _closeButtons)
                closeButton.OnClick += OnCloseClick;
        }

        protected virtual void OnDestroy()
        {
            foreach (var closeButton in _closeButtons)
                closeButton.OnClick -= OnCloseClick;
        }

        protected virtual void OnCloseClick() => 
            Close();

        [Button, HideInEditorMode]
        public virtual void Open()
        {
            DisableInteraction();
            EnableWindow();
            IsOpened = true;
            OnOpen?.Invoke(this);
            OnOpenUnityEvent?.Invoke();
            OpenAnimation(_completeOpening);
        }

        [Button, HideInEditorMode]
        public virtual void Close()
        {
            DisableInteraction();

            if (IsOpened)
            {
                IsOpened = false;
                OnClose?.Invoke(this);
                OnCloseUnityEvent?.Invoke();
                CloseAnimation(_completeClosing);
            }
            else
                gameObject.Off();
        }
        

        protected virtual void OpenAnimation(Action callback) => 
            callback?.Invoke();

        protected virtual void CloseAnimation(Action callback) =>
            callback?.Invoke();

        private void EnableWindow() =>
            gameObject.On();

        protected virtual void CompleteClosing()
        {
            gameObject.Off();
            OnClosed?.Invoke(this);
            OnClosedUnityEvent?.Invoke();
        }

        protected virtual void CompleteOpening()
        {
            OnOpened?.Invoke(this);
            OnOpenedUnityEvent?.Invoke();
            EnableInteraction();
        }

        private void EnableInteraction()
        {
            if (!_blockInteractionWhileAnimated) return;
            _canvasGroup.interactable = true;
        }

        private void DisableInteraction()
        {
            if (!_blockInteractionWhileAnimated) return;
            _canvasGroup.interactable = false;
        }
    }
}