using System;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Products.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class BadgeView : MonoBehaviour
    {
        [Serializable]
        public class BadgeVisualSetup
        {
            [SerializeField] private TextMeshProUGUI _text;
            [SerializeField] private TextMeshProUGUI _value;
            [SerializeField] private GameObject _root;
            [SerializeField] private string _id;

            public string Id => _id;

            public void Enable(VisualData data)
            {
                _root.gameObject.SetActive(true);
                if(_text != null)
                    _text.SetText(data.BadgeText);
                if(_value != null)
                    _value.SetText(data.BadgeValue);
            }

            public void Disable() => 
                _root.gameObject.SetActive(false);
        }
        
        [SerializeField] private BadgeVisualSetup[] _setups;

        private void Awake() => 
            Hide();

        public void Show(VisualData data)
        {
            foreach (var setup in _setups)
            {
                if (string.Equals(setup.Id, data.BadgeId))
                    setup.Enable(data);
                else
                    setup.Disable();
            }
        }
        
        public void Hide()
        {
            foreach (var setup in _setups)
                setup.Disable();   
        }
    }
}