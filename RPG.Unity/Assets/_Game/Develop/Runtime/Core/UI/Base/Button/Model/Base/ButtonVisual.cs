using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base
{
    [Serializable]
    internal class ButtonVisual
    {
        private enum ButtonSwitchType
        {
            Color = 0,
            Object = 1,
        }
        [SerializeField] private ButtonSwitchType _switchType = ButtonSwitchType.Color; 
        [HideIf("@this._switchType == ButtonSwitchType.Color")]
        public GameObject InteractableVisual;
        [HideIf("@this._switchType == ButtonSwitchType.Color")]
        public GameObject NonInteractableVisual;
        
        [HideIf("@this._switchType == ButtonSwitchType.Object")]
        public Graphic Graphic;
        [HideIf("@this._switchType == ButtonSwitchType.Object")]
        public Color Enable;
        [HideIf("@this._switchType == ButtonSwitchType.Object")]
        public Color Disable;

        public void SetInteractable(bool value)
        {
            switch (_switchType)
            {
                case ButtonSwitchType.Color:
                    Graphic.color = value ? Enable : Disable;
                    break;
                case ButtonSwitchType.Object:
                    InteractableVisual.SetActive(value);
                    NonInteractableVisual.SetActive(!value);
                    break;
            }
        }
    }
}