using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.WindowManagement.Contract;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class BlessingButton : MonoBehaviour 
    {
        [SerializeField, Required] private BaseButton _button;
        [SerializeField] private Transform _visual;
        [SerializeField] private List<BlessingPreview> _previews;

        [Inject] private IWindowService _windowService;
        [Inject] private BlessingService _blessingService;

        public Transform Visual => _visual;

        private void Awake()
        {
            Initialize();
            _button.OnClick += OnButtonClicked;
        }

        private void OnDestroy() =>
            _button.OnClick -= OnButtonClicked;

        private void Initialize()
        {
            for (int i = 0; i < _previews.Count; i++)
            {
                if(i >= _blessingService.Blessings.Count) break;
                _previews[i].Setup(_blessingService.Blessings[i]);
            }
        }

        private async void OnButtonClicked() => 
            await _windowService.OpenAsync<BlessingWindow>();
    }
}