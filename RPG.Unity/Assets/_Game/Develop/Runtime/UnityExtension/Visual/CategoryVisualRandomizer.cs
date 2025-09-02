using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.UnityExtension.Visual
{
    public class CategoryVisualRandomizer : VisualRandomizer
    {
        [SerializeField] public List<GroupVisual> _groups;
        [SerializeField] public List<GameObject> _allVisalObjects;
        
        private GroupVisual _current;

        private void OnValidate()
        {
            _allVisalObjects.Clear();
            
            foreach (var obj in from @group in _groups
                     from category in @group.Category
                     from variant in category.Variants
                     from obj in variant.Objects
                     where obj != null select obj)
            {
                if(!_allVisalObjects.Contains(obj))
                    _allVisalObjects.Add(obj);
            }
        }

        private void Awake()
        {
            foreach (var visualObject in _allVisalObjects) 
                visualObject.SetActive(false);
        }

        protected override void ChangeVisual()
        {
            _current?.Hide();
            _current = _groups.GetRandomElement();
            _current.Show();
        }
    }

    [Serializable]
    public class GroupVisual
    {
        [SerializeField] private string _name;
        [SerializeField] private List<CategoryVisual> _category;

        public List<CategoryVisual> Category => _category;
        
        private List<GameObject> _selectedObjects = new();
        
        public void Show()
        {
            foreach (var categoryVisual in _category)
            {
                var variant = categoryVisual.Variants.WeightedRandomElement();
                foreach (var variantObject in variant.Objects)
                {
                    if(variantObject == null) continue;
                    variantObject.SetActive(true);
                    _selectedObjects.Add(variantObject);   
                }
            }
        }

        public void Hide()
        {
            foreach (var selectedObject in _selectedObjects) 
                selectedObject.gameObject.SetActive(false);
            
            _selectedObjects.Clear();
        }
    }

    [Serializable]
    public class CategoryVisual
    {
        public string Name;
        public List<VariantVisual> Variants;
    }

    [Serializable]
    public class VariantVisual : IWeightedRandom
    {
        public int Weight = 1;
        public int RandomWeight => Weight;
        public List<GameObject> Objects;
    }
}