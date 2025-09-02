using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Accent.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class AccentSettingsHolder : MonoBehaviour
    {
        [SerializeField] private RectTransform _target;
        [SerializeField] private RectTransform _parent;
        [SerializeField] private AccentSettings _settings;

        public RectTransform Target => _target;
        public RectTransform Parent => _parent;
        public AccentSettings Settings => _settings;

        public void SetParent(RectTransform parent)
        {
            _parent = parent;
        }
    }
}