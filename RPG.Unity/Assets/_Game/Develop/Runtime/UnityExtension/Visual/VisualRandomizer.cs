using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.UnityExtension.Visual
{
    [HideMonoScript]
    public abstract class VisualRandomizer : MonoBehaviour
    {
        [SerializeField] private bool _changeOnEnable = true;

        private void OnEnable()
        {
            if(!_changeOnEnable) return;
            ChangeVisual();
        }

        protected abstract void ChangeVisual();
    }
}