using System.Collections.Generic;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.UnityExtension.Extensions
{
    public static class GameObjectExtension
    {
        public static void Off(this GameObject gameObject)
        {
            if(!gameObject.activeSelf) return;
            gameObject.SetActive(false);
        }

        public static void On(this GameObject gameObject)
        {
            if(gameObject.activeSelf) return;
            gameObject.SetActive(true);
        }

        public static T FindObjectOfTypeInObjectContext<T>(this GameObject gameObject) where T : Component
        {
            var componentsOfType = Object.FindObjectsOfType<T>();
            
            foreach (var component in componentsOfType)
            {
                if(component.gameObject.scene != gameObject.scene) continue;
                return component;
            }
            
            return null;
        }
        
        public static List<T> FindObjectsOfTypeInObjectContext<T>(this GameObject gameObject) where T : Component
        {
            var componentsOfType = Object.FindObjectsOfType<T>();

            var result = new List<T>();
            foreach (var component in componentsOfType)
            {
                if(component.gameObject.scene != gameObject.scene) continue;
                result.Add(component);
            }
            
            return result;
        }
    }
}