using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public abstract class PhonePage : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        
        private Action _completeOpening;
        private Action _completeClosing;

        public event Action OnShown;
        public event Action OnHidden;
        
        public virtual void Initialize()
        {
            _completeOpening = CompleteOpening;
            _completeClosing = CompleteClosing;
            DisableIfActive();
        }

        public virtual void Show()
        {
            DisableInteraction();
            gameObject.SetActive(true);
            OpenAnimation(_completeOpening);
        }

        public virtual void Hide()
        {
            DisableInteraction();
            CloseAnimation(_completeClosing);
        }

        private void DisableIfActive()
        {
            if(gameObject.activeSelf)
                gameObject.SetActive(false);
        }

        public abstract void CloseSignal();
        
        protected virtual void OpenAnimation(Action callback) => 
            callback?.Invoke();
        
        protected virtual void CloseAnimation(Action callback) =>
            callback?.Invoke();
        
        protected virtual void CompleteClosing()
        {
            gameObject.SetActive(false);
            OnHidden?.Invoke();
        }

        protected virtual void CompleteOpening()
        {
            OnShown?.Invoke();
            EnableInteraction();
        }
        
        private void EnableInteraction() => 
            _canvasGroup.interactable = true;

        private void DisableInteraction() => 
            _canvasGroup.interactable = false;
    }
}