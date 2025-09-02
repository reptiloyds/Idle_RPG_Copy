using System;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View.Components;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class SubModeCardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private SubModeEnterButtons _enterButtons;
        [SerializeField] private SubModeEnterResourceView _enterResourceView;
        [SerializeField] private SubModeRewardPreview _subModeRewardPreview;
        [SerializeField] private Image _image;
        [SerializeField] private GameObject _blockObject;
        [SerializeField] private TextMeshProUGUI _unlockConditionText;

        private SubMode _subMode;
        public BaseButton EnterButton => _enterButtons.DefaultButton;
        public event Action<SubModeCardView> OnEnterClick;

        [Inject] private IAssetProvider _assetProvider;
        
        private void Awake() =>
            _enterButtons.OnClicked += OnClick;

        private void OnDestroy()
        {
            if (_subMode != null)
            {
                _subMode.EnterResource.OnChange -= RedrawEnterResourceView;
                _subMode.OnBonusEnterSpent -= RedrawEnterResourceView;
                _subMode.OnNameChanged -= RedrawName;
            }
            _enterButtons.OnClicked -= OnClick;
        }

        private void OnClick() => 
            OnEnterClick?.Invoke(this);

        public void Setup(SubMode subMode)
        {
            _subMode = subMode;
            _enterButtons.Setup(_subMode, true);
            _blockObject.SetActive(!_subMode.IsUnlocked);
            if (!_subMode.IsUnlocked)
            {
                _unlockConditionText.SetText(_subMode.Condition);
                _subMode.OnUnlocked += OnDungeonUnlocked;
            }
            
            _subMode.EnterResource.OnChange += RedrawEnterResourceView;
            _subMode.OnBonusEnterSpent += RedrawEnterResourceView;
            _subMode.OnNameChanged += RedrawName;

            RedrawName();
            _subModeRewardPreview.Redraw(_subMode);
            RedrawDynamicData();
            SetupBackgroundImage().Forget();
        }

        private void RedrawName() => 
            _nameText.SetText(_subMode.Name);

        private async UniTaskVoid SetupBackgroundImage() => 
            _image.sprite = await _assetProvider.LoadAssetAsync<Sprite>(_subMode.BackgroundRef, false);

        private void OnDungeonUnlocked(IUnlockable unlockable)
        {
            _subMode.OnUnlocked -= OnDungeonUnlocked;
            _blockObject.SetActive(false);
        }
        
        private void RedrawEnterResourceView()
        {
            _enterResourceView.Redraw(_subMode);
            _enterButtons.Redraw();
        }

        private void RedrawDynamicData()
        {
            _enterButtons.Redraw();
            _enterResourceView.Redraw(_subMode);
        }
    }
}