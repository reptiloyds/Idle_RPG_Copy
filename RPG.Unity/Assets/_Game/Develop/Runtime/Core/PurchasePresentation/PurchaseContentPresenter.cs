using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.PurchasePresentation
{
    [DisallowMultipleComponent, HideMonoScript]
    public class PurchaseContentPresenter : MonoBehaviour
    {
        [Serializable]
        private class ContentSetup
        {
            public GameObject Object;
            public Image Image;
        }

        [Serializable]
        private class BackgroundSetup
        {
            public PurchaseContentBackground Type;
            public GameObject BackObject;
        }
        
        [SerializeField] private RectTransform _visualContainer;
        [SerializeField] private List<BackgroundSetup> _backgrounds;
        [SerializeField] private ContentSetup _singleSetup;
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private List<ContentSetup> _multipleSetups;
        [SerializeField] private List<Graphic> _colorGraphics;

        public void Redraw(List<Sprite> sprites, Color color, PurchaseContentBackground contentBackground, string labelText = null)
        {
            if (sprites.Count == 1)
            {
                _singleSetup.Object.SetActive(true);
                _singleSetup.Image.sprite = sprites[0];
                foreach (var setup in _multipleSetups)
                    setup.Object.SetActive(false);
            }
            else
            {
                _singleSetup.Object.SetActive(false);
                for (var i = 0; i < _multipleSetups.Count; i++)
                {
                    if(sprites.Count <= i) break;
                    _multipleSetups[i].Object.SetActive(true);
                    _multipleSetups[i].Image.sprite = sprites[i];
                }   
            }

            foreach (var graphic in _colorGraphics)
            {
                var alpha = graphic.color.a;
                graphic.color = new Color(color.r, color.g, color.b, alpha);
            }
            
            if (string.IsNullOrEmpty(labelText))
            {
                _label.gameObject.SetActive(false);
                _visualContainer.offsetMin = Vector2.zero;
            }
            else
            {
                _visualContainer.offsetMin = new Vector2(0, _label.rectTransform.rect.height);
                _label.SetText(labelText);
                _label.gameObject.SetActive(true);
            }

            foreach (var backgroundSetup in _backgrounds) 
                backgroundSetup.BackObject.SetActive(backgroundSetup.Type == contentBackground);
        }
    }
}