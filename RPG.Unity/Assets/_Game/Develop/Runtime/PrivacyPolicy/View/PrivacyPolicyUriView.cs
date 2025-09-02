using System;
using PleasantlyGames.RPG.Runtime.PrivacyPolicy.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.PrivacyPolicy.View
{
    public class PrivacyPolicyUriView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private Button _button;

        private PrivacyPolicyUri _uri;
        
        public event Action<PrivacyPolicyUri> OnClicked; 
        
        private void Awake() => 
            _button.onClick.AddListener(OnClick);

        private void OnDestroy() => 
            _button.onClick.RemoveAllListeners();

        private void OnClick() => 
            OnClicked?.Invoke(_uri);

        public void Setup(PrivacyPolicyUri uri)
        {
            _uri = uri;
            _nameText.SetText(_uri.UriName);
        }
    }
}