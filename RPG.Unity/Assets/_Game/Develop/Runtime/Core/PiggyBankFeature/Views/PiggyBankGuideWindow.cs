using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Views
{
    public class PiggyBankGuideWindow : BaseWindow
    {
        [SerializeField, Required] private RectTransform _firstStageContainer;
        [SerializeField, Required] private RectTransform _secondStageContainer;
        [SerializeField, Required] private BaseButton _closeFirstStageButton;
        [SerializeField, Required] private BaseButton _closeWindow;
        [SerializeField] private float _delay;
        [SerializeField] private List<RectTransform> _firstStageElements = new List<RectTransform>();
        [SerializeField] private List<RectTransform> _secondStageElements = new List<RectTransform>();

        private void Start()
        {
            _closeFirstStageButton.OnClick += MoveToSecondStage;
            _closeWindow.OnClick += OnCloseClick;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _closeFirstStageButton.OnClick -= MoveToSecondStage;
            _closeWindow.OnClick -= OnCloseClick;
        }

        public override void Open()
        {
            base.Open();
            Reset();
        }

        private void Reset()
        {
            _firstStageContainer.gameObject.Off();
            _secondStageContainer.gameObject.Off();

            foreach (var element in _firstStageElements)
                element.gameObject.Off();
            
            foreach (var element in _secondStageElements)
                element.gameObject.Off();
            
            _firstStageContainer.gameObject.On();
            AnimateElements(_firstStageElements).Forget();
        }

        private void MoveToSecondStage()
        {
            _firstStageContainer.gameObject.Off();
            _secondStageContainer.gameObject.On();
            AnimateElements(_secondStageElements).Forget();
        }

        private async UniTask AnimateElements(List<RectTransform> elements)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (i == 0)
                {
                    elements[i].gameObject.On();
                    continue;
                }

                await UniTask.Delay(TimeSpan.FromSeconds(_delay), ignoreTimeScale: true);
                elements[i].gameObject.On();
            }
        }
    }
}