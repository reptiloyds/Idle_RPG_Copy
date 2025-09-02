using PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Products.View.Periodic
{
    [DisallowMultipleComponent, HideMonoScript]
    public class PeriodicTimer : MonoBehaviour
    {
        [SerializeField] private UITimer _timer;
        [SerializeField] private RectTransform _rect;

        public RectTransform Rect => _rect;

        public void Listen(ReadOnlyReactiveProperty<float> cooldown, bool toEnd = true) => 
            _timer.Listen(cooldown, toEnd);
    }
}