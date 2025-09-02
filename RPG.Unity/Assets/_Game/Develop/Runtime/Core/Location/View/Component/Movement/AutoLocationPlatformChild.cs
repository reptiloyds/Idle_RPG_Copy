using PleasantlyGames.RPG.Runtime.Core.Location.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Location.View.Component.Movement
{
    [DisallowMultipleComponent, HideMonoScript]
    public class AutoLocationPlatformChild : MonoBehaviour
    {
        [Inject] private LocationFactory _locationFactory;

        private void OnEnable() => TryChangeParent();

        private void TryChangeParent()
        {
            if(_locationFactory == null) return;
            var location = _locationFactory.Location;
            if(location == null) return;
            location.AppendChild(transform);
        }
    }
}