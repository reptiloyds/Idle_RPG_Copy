using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UnitRendering
{
    [DisallowMultipleComponent, HideMonoScript]
    public class UnitRenderRequest : MonoBehaviour
    {
        [Inject] private UnitRenderer _unitRenderer;
        
        private void OnEnable()
        {
            if(_unitRenderer != null)
                _unitRenderer.AddRequest(this);
        }

        private void OnDisable()
        {
            if(_unitRenderer != null)
                _unitRenderer.RemoveRequest(this);
        }
    }
}