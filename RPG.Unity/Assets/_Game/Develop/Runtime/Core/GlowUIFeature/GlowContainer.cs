using System;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.GlowUIFeature
{
    public class GlowContainer : MonoBehaviour
    {
        [SerializeField, ReadOnly] private List<Image> _images = new List<Image>();

        private void OnValidate()
        {
            Cache();
        }

        [Button]
        private void Cache()
        {
            _images = GetComponentsInChildren<Image>().ToList();
        }

        public void SetColor(Color color)
        {
            foreach (Image image in _images)
                image.color = color;
        }

        public void Show()
        {
            gameObject.On();
        }

        public void Hide()
        {
            gameObject.Off();
        }
    }
}