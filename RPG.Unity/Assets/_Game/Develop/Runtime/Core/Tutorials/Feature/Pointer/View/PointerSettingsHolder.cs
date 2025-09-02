using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Pointer.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class PointerSettingsHolder : MonoBehaviour
    {
        [SerializeField] private PointerSettings _settings;
        
        public PointerSettings Settings => _settings;

        public void SetTarget(RectTransform target)
        {
            _settings.Target = target;
        }
    }
}