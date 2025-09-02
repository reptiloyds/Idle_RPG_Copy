using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Pause.Contract;
using PleasantlyGames.RPG.Runtime.Pause.Type;
using PleasantlyGames.RPG.Runtime.PrivacyPolicy.Model;
using PleasantlyGames.RPG.Runtime.Save.Contracts;
using PleasantlyGames.RPG.Runtime.Utilities.ApplicationCloser.Contract;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.PrivacyPolicy.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class PrivacyPolicyOpenTriggerView : MonoBehaviour
    {
        [SerializeField] private BaseButton _button;
        [SerializeField] private CanvasGroup _group;
        
        [Inject] private PrivacyPolicyService _service;
        [Inject] private ISaveService _saveService;
        [Inject] private IPauseService _pauseService;
        [Inject] private IApplicationCloser _applicationCloser;

        private void Awake() => 
            _button.OnClick += OnClick;

        private void OnDestroy() => 
            _button.OnClick -= OnClick;

        private async void OnClick()
        {
            _group.interactable = false;
            _button.SetInteractable(false);
            _pauseService.Pause(PauseType.Time);
            var previousResult = _service.IsAccepted;
            await _service.ShowPrivacyPolicyAsync();
            if (previousResult == _service.IsAccepted)
                Continue();
            else
                RestartApplication();
        }

        private async void RestartApplication()
        {
            await _saveService.SaveAndLoadToCloudAsync();
            _applicationCloser.Close();
        }

        private void Continue()
        {
            _button.SetInteractable(true);
            _pauseService.Continue(PauseType.Time);
            _group.interactable = true;
        }
    }
}