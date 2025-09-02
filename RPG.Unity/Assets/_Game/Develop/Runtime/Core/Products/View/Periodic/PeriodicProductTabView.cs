using System;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Products.Model;
using PleasantlyGames.RPG.Runtime.Core.Products.Model.Periodic;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Products.View.Periodic
{
    [DisallowMultipleComponent, HideMonoScript]
    public class PeriodicProductTabView : MonoBehaviour
    {
        [SerializeField] private PeriodicType _type;

        [ShowInInspector, HideInEditorMode, ReadOnly]
        private List<RectTransform> _parts = new();
        [ShowInInspector, HideInEditorMode, ReadOnly]
        private PeriodicLabel _label;
        [ShowInInspector, HideInEditorMode, ReadOnly]
        private PeriodicTimer _timer;
        private RectTransform _firstRect;
        private RectTransform _lastRect;

        [Inject] private ITranslator _translator;
        [Inject] private ProductService _service;
        
        public PeriodicType Type => _type;
        public float StartPosition => _firstRect.anchoredPosition.y;

        public void SetLabel(PeriodicLabel label)
        {
            _label = label;
            _label.SetText(_translator.Translate(TranslationConst.PurchasePrefix + _type));
            _parts.Add(label.Rect);
            _firstRect ??= label.Rect;
        }

        public void SetTimer(PeriodicTimer timer)
        {
            _timer = timer;
            _timer.Listen(_service.PeriodicProducts.Refreshes[_type], false);
            _parts.Add(timer.Rect);
            _firstRect ??= timer.Rect;
        }

        public void AddPart(RectTransform rectTransform)
        {
            _firstRect ??= rectTransform;
            _parts.Add(rectTransform);
        }

        public void Initialize() => 
            _lastRect = _parts.LastOrDefault();

        public RectTransform GetFirstRect() =>
            _firstRect;

        public RectTransform GetLastRect() => 
            _lastRect;
    }
}