using System;
using PleasantlyGames.RPG.Runtime.BonusAccess.View;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Model;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class BlessingView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _bonusDescription;
        [SerializeField] private TextMeshProUGUI _bonus;
        [SerializeField] private TextMeshProUGUI _duration;
        [SerializeField] private GameObject _upArrow;
        [SerializeField] private GameObject _downArrow;
        [SerializeField] private BonusAccessButton _bonusButton;
        [SerializeField] private LevelProgressionView _levelProgressionView;
        [SerializeField] private GameObject _enableBackground;
        [SerializeField] private GameObject _disableBackground;

        [Inject] private ITranslator _translator;
        
        private Blessing _model;
        public Blessing Model => _model;

        public event Action<BlessingView> OnRewarded;

        private void Awake()
        {
            _bonusButton.OnExecuted += BonusExecuted;
        }

        private void BonusExecuted() => 
            OnRewarded?.Invoke(this);

        private void OnDestroy()
        {
            _bonusButton.OnExecuted -= BonusExecuted;
            ClearBlessing();
        }

        public void Setup(Blessing blessing)
        {
            ClearBlessing();
            _model = blessing;
            _bonusButton
                .SetLabelText(_translator.Translate(TranslationConst.Receive))
                .SetFreeAccess(_model.FreeActivation)
                .SetCooldown(_model.Duration)
                .Build();
            
            _model.OnEnabled += ModelStateChanged;
            _model.OnDisabled += ModelStateChanged;
            _model.OnProgressionChanged += RedrawDynamicData;

            _image.sprite = _model.Sprite;
            _name.SetText(_model.Name);
            _bonusDescription.SetText(_model.BonusDescription);
            _duration.SetText(_model.GetDuration());
            _upArrow.SetActive(_model.IsIncreaseEffect);
            _downArrow.SetActive(!_model.IsIncreaseEffect);

            RedrawDynamicData();
        }

        private void ClearBlessing()
        {
            if (_model == null) return;
            _model.OnEnabled -= ModelStateChanged;
            _model.OnDisabled -= ModelStateChanged;
            _model.OnProgressionChanged -= RedrawDynamicData;
            _model = null;
        }

        private void ModelStateChanged(Blessing blessing) =>
            RedrawDynamicData();

        private void RedrawDynamicData()
        {
            _bonus.SetText(_model.GetBonusValue());
            
            _enableBackground.SetActive(_model.IsActive.CurrentValue);
            _disableBackground.SetActive(!_model.IsActive.CurrentValue);

            _levelProgressionView.Redraw(_model.Level, _model.IsLevelMax, _model.Progression,
                _model.TargetProgression);
        }
    }
}