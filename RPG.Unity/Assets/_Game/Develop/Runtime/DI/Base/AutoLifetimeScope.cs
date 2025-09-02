using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PleasantlyGames.RPG.Runtime.DI.Attributes;
using PleasantlyGames.RPG.Runtime.UnityExtension;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.DI.Base
{
    [HideMonoScript]
    public abstract class AutoLifetimeScope : LifetimeScope, IEditorUpdatable
    {
        private enum SearchType
        {
            InScene = 0,
            InObject = 1,
        }
        
        [SerializeField] private SearchType _searchType = SearchType.InScene;
        [SerializeField, ReadOnly] protected List<AutoFeatureScope> _autoFeatureScopes;
        protected const bool DestroyFeatureScopeAfterBind = true;
        
        void IEditorUpdatable.InvokeUpdate() => AutoFill();

        protected override void Configure(IContainerBuilder builder)
        {
            foreach (var featureScope in _autoFeatureScopes)
                featureScope.Configure(builder);

            if (DestroyFeatureScopeAfterBind) ClearFeatureInstallers();
        }

        [Button]
        protected virtual void AutoFill()
        {
            var fields = GetAllFields(GetType()).ToList();
            
            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttribute<AutoFillAttribute>(); 
                if (attribute == null) continue;
                object component = null;


                switch (_searchType)
                {
                    case SearchType.InScene:
                        var relevantObjects = FindObjectsOfType(field.FieldType, true);
                        foreach (var relevantObject in relevantObjects)
                        {
                            if(attribute.StrictType && relevantObject.GetType() != field.FieldType) continue;
                            var monoBehaviour = relevantObject as Component;
                            if(monoBehaviour != null && monoBehaviour.gameObject.scene != gameObject.scene) continue;
                    
                            component = relevantObject;
                            break;
                        }
                        break;
                    default:
                    case SearchType.InObject:
                        var relevantComponents = gameObject.GetComponentsInChildren(field.FieldType, true);
                        foreach (var relevantComponent in relevantComponents)
                        {
                            if(attribute.StrictType && relevantComponent.GetType() != field.FieldType) continue;
                            if(relevantComponent != null && relevantComponent.gameObject.scene != gameObject.scene) continue;
                    
                            component = relevantComponent;
                            break;
                        }
                        break;
                }
                
                field.SetValue(this, component);
            }
            
            UpdateFeatureScopes();
            CheckForDuplicates();
            
            #if UNITY_EDITOR
            EditorUtility.SetDirty(gameObject);
            #endif
        }

        private void UpdateFeatureScopes()
        {
            autoInjectGameObjects.Clear();
            _autoFeatureScopes = GetComponentsInChildren<AutoFeatureScope>(true).ToList();
            foreach (var featureScope in _autoFeatureScopes)
            {
                featureScope.AutoFill();
                
                #if UNITY_EDITOR
                EditorUtility.SetDirty(featureScope);
                #endif
            }
            
            _autoFeatureScopes.ForEach(item => autoInjectGameObjects.AddRange(item.AutoInjectGameObjects));

            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            #endif
        }
        
        protected void AddAutoInjectedObject<T>(bool all = false) where T : Component
        {
            if (all)
            {
                switch (_searchType)
                {
                    case SearchType.InScene:
                        var sceneComponents = gameObject.FindObjectsOfTypeInObjectContext<T>();
                        foreach (var component in sceneComponents) 
                            autoInjectGameObjects.Add(component.gameObject);  
                        break;
                    case SearchType.InObject:
                        var objectComponents = gameObject.GetComponentsInChildren<T>(true);
                        foreach (var component in objectComponents) 
                            autoInjectGameObjects.Add(component.gameObject);  
                        break;
                }
            }
            else
            {
                switch (_searchType)
                {
                    case SearchType.InScene:
                        var sceneComponents = gameObject.GetComponentInChildren<T>();
                        if (sceneComponents != null)
                            autoInjectGameObjects.Add(sceneComponents.gameObject);
                        break;
                    case SearchType.InObject:
                        var objectComponents = gameObject.GetComponentInChildren<T>();
                        if (objectComponents != null)
                            autoInjectGameObjects.Add(objectComponents.gameObject);
                        break;
                }
            }
        }

        private void CheckForDuplicates()
        {
            var duplicates = _autoFeatureScopes.GroupBy(x => x.GetType())
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();
            if (duplicates.Any()) {
                Logger.LogError("Duplicates: " + string.Join(", ", duplicates.Select(t => t.Name)));
            } 
        }

        protected void ClearFeatureInstallers()
        {
            foreach (var featureScope in _autoFeatureScopes)
            {
                Destroy(featureScope);
            }
            _autoFeatureScopes.Clear();
        }

        private static IEnumerable<FieldInfo> GetAllFields(Type type)
        {
            if (type == null)
                return Enumerable.Empty<FieldInfo>();
            
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return fields.Concat(GetAllFields(type.BaseType));
        }
    }
}