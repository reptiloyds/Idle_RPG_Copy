using System.Collections.Generic;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Hub.Buttons
{
    public sealed class ObjectsHubButton : HubButton
    {
        [SerializeField] private List<GameObject> _enableObjects;
        [SerializeField] private List<GameObject> _disableObjects;

        private void Start()
        {
            foreach (var enableObject in _enableObjects) 
                enableObject.SetActive(false);
            foreach (var disableObject in _disableObjects) 
                disableObject.SetActive(true);
        }

        protected override void Click()
        {
            base.Click();

            if (IsActive)
            {
                foreach (var enableObject in _enableObjects) 
                    enableObject.SetActive(false);
                foreach (var disableObject in _disableObjects) 
                    disableObject.SetActive(true);
                DisableCloseVisual();
                TriggerAutoDeactivate();   
            }
            else
            {
                foreach (var enableObject in _enableObjects) 
                    enableObject.SetActive(true);
                foreach (var disableObject in _disableObjects) 
                    disableObject.SetActive(false);
                EnableCloseVisual();
                TriggerAutoActivate();
            }

            IsActive = !IsActive;
        }

        public override void Deactivate()
        {
            foreach (var enableObject in _enableObjects) 
                enableObject.SetActive(false);
            foreach (var disableObject in _disableObjects) 
                disableObject.SetActive(true);
            IsActive = false;
            DisableCloseVisual();
            TriggerAutoDeactivate();
        }
    }
}