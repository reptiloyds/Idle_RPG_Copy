using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.PrivacyPolicy.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.PrivacyPolicy.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class PrivacyPolicyView : MonoBehaviour
    {
        [SerializeField] private RectTransform _privacyPolicyUriContainer;
        [SerializeField] private PrivacyPolicyUriView _uriView;
        [SerializeField] private Button _acceptButton;
        [SerializeField] private Button _rejectButton;

        private List<PrivacyPolicyUriView> _views;
        
        public event Action OnAccepted; 
        public event Action OnRejected;
        public event Action<PrivacyPolicyUri> OnUriClicked; 

        private void Awake()
        {
            _acceptButton.onClick.AddListener(OnAcceptClicked);
            _rejectButton.onClick.AddListener(OnRejectClicked);
        }

        private void OnDestroy()
        {
            foreach (var view in _views) 
                view.OnClicked -= OnClicked;
            
            _acceptButton.onClick.RemoveAllListeners();
            _rejectButton.onClick.RemoveAllListeners();
        }

        private void OnAcceptClicked() => 
            OnAccepted?.Invoke();

        private void OnRejectClicked() => 
            OnRejected?.Invoke();

        public void Setup(PrivacyPolicyUri[] uris)
        {
            _views = new List<PrivacyPolicyUriView>(uris.Length);
            foreach (var uri in uris)
            {
                var view = Instantiate(_uriView, _privacyPolicyUriContainer);
                view.OnClicked += OnClicked;
                view.Setup(uri);
                _views.Add(view);
            }
        }

        private void OnClicked(PrivacyPolicyUri uri) => 
            OnUriClicked?.Invoke(uri);
    }
}