using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Resource.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class ResourcePresenterPopup : MonoBehaviour
    {
        [SerializeField] private RectTransform _context;
        [SerializeField] private List<ResourceType> _resources;
        [SerializeField] private bool _popupOnEnable;
        [SerializeField] private bool _revertOnDisable;
        
        [Inject] private ResourceViewService _resourceViewService;

        private ResourcePopupRequest _request;

        private void Awake() => 
            _request = new ResourcePopupRequest(_resources, _context.parent);

        private void OnEnable()
        {
            if(_popupOnEnable)
                Popup();
        }

        private void OnDisable()
        {
            if(_revertOnDisable)
                CancelPopup();
        }

        [Button, HideInEditorMode]
        public void Popup() => 
            _resourceViewService.Popup(_request, this);

        [Button, HideInEditorMode]
        public void CancelPopup() => 
            _resourceViewService.CancelPopup(this);
    }
}