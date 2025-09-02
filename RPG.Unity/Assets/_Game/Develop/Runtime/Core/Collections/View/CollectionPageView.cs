using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Collections.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CollectionPageView : MonoBehaviour
    {
        [SerializeField] private ItemType _type;
        [SerializeField] private RectTransform _container;

        private readonly List<CollectionView> _views = new();
        
        public ItemType Type => _type;
        public RectTransform Container => _container;

        public void AppendView(CollectionView view) => 
            _views.Add(view);
    }
}