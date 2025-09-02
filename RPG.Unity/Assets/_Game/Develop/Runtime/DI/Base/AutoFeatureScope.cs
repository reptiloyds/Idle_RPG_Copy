using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PleasantlyGames.RPG.Runtime.DI.Attributes;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.DI.Base
{
    [HideMonoScript]
    public abstract class AutoFeatureScope : MonoBehaviour
    {
        [InfoBox("Optional")]
        
        [SerializeField, HideInInspector] private List<GameObject> _autoInjectGameObjects = new ();

        public List<GameObject> AutoInjectGameObjects => _autoInjectGameObjects;

        public virtual void Configure(IContainerBuilder builder)
        {
        }

        private void OnValidate()
        {
            if (enabled) enabled = false;
        }

        public virtual void AutoFill()
        {
            var fields = GetAllFields(GetType()).ToList();
            
            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttribute<AutoFillAttribute>(); 
                if (attribute == null) continue;
                object component = null;
                
                var relevantObjects = FindObjectsOfType(field.FieldType, true);
                foreach (var relevantObject in relevantObjects)
                {
                    if(attribute.StrictType && relevantObject.GetType() != field.FieldType) continue;
                    var monoBehaviour = relevantObject as Component;
                    if(monoBehaviour != null && monoBehaviour.gameObject.scene != gameObject.scene) continue;
                    
                    component = relevantObject;
                    break;
                }
                
                field.SetValue(this, component);
            }
            
            _autoInjectGameObjects.Clear();
        }
        
        private static IEnumerable<FieldInfo> GetAllFields(Type type)
        {
            if (type == null)
                return Enumerable.Empty<FieldInfo>();
            
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return fields.Concat(GetAllFields(type.BaseType));
        }

        protected void AddAutoInjectedObject<T>(bool all = false) where T : Component
        {
            if (all)
            {
                var components = gameObject.FindObjectsOfTypeInObjectContext<T>();
                foreach (var component in components) 
                    AutoInjectGameObjects.Add(component.gameObject);   
            }
            else
            {
                var component = gameObject.FindObjectOfTypeInObjectContext<T>();
                if (component != null)
                    AutoInjectGameObjects.Add(component.gameObject);
            }
        }
    }
}