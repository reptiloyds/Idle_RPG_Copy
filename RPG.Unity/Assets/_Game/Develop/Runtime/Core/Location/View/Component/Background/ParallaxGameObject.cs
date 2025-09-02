using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Location.View.Component.Background
{
    public class ParallaxGameObject : BaseParallax
    {
        [SerializeField] private GameObject _viewPrefab;

        protected override GameObject GetViewObject() => 
            Instantiate(_viewPrefab, transform);
    }
}
