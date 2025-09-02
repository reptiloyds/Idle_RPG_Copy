using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Message.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Branches.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class BranchSwitchView : MonoBehaviour
    {
        [SerializeField, Required] private TextMeshProUGUI _description;
        [SerializeField, Required] private Image _leftBranchImage;
        [SerializeField] private Color _blockButtonColor;
        [SerializeField, Required] private BaseButton _leftButton;
        [SerializeField, Required] private Image _leftButtonImage;
        [SerializeField, Required] private GameObject _leftBlock;
        [SerializeField, Required] private Image _rightBranchImage;
        [SerializeField, Required] private BaseButton _rightButton;
        [SerializeField, Required] private Image _rightButtonImage;
        [SerializeField, Required] private GameObject _rightBlock;
        
        [Inject] private BranchService _branchService;
        [Inject] private ITranslator _translator;
        [Inject] private MessageBuffer _messageBuffer;

        private void Awake()
        {
            _leftButton.OnClick += OnLeftClick;
            _leftButton.OnFailClick += OnLeftFailClick;
            _rightButton.OnClick += OnRightClick;
            _rightButton.OnFailClick += OnRightFailClick;
            
            _branchService.SwitchBranch += Redraw;
            _branchService.BranchUnlock += OnBranchUnlock;
            Redraw();
        }

        private void OnDestroy()
        {
            _leftButton.OnClick -= OnLeftClick;
            _leftButton.OnFailClick -= OnLeftFailClick;
            _rightButton.OnClick -= OnRightClick;
            _rightButton.OnFailClick -= OnRightFailClick;
            if (_branchService != null)
            {
                _branchService.BranchUnlock -= OnBranchUnlock;
                _branchService.SwitchBranch -= Redraw;
            }
        }

        private void OnBranchUnlock(Branch branch) => 
            Redraw();

        private void OnLeftClick()
        {
            var newBranch = GetBranch(-1);
            _branchService.ChangeBranch(newBranch);
        }

        private void OnLeftFailClick()
        {
            var newBranch = GetBranch(-1);
            _messageBuffer.Send(newBranch.Condition);
        }

        private void OnRightClick()
        {
            var newBranch = GetBranch(+1);
            _branchService.ChangeBranch(newBranch);
        }

        private void OnRightFailClick()
        {
            var newBranch = GetBranch(+1);
            _messageBuffer.Send(newBranch.Condition);
        }

        private Branch GetBranch(int indexDelta)
        {
            var currentIndex = _branchService.GetSelectedBranchIndex();
            return _branchService.Branches[currentIndex + indexDelta];
        }

        private void Redraw()
        {
            var branches = _branchService.Branches;
            var currentBranch = _branchService.GetSelectedBranch();
            var currentIndex = _branchService.GetSelectedBranchIndex();
            
            _description.SetText(_translator.Translate($"{currentBranch.Id}{TranslationConst.BranchPostfix}"));

            if (currentIndex > 0)
            {
                var leftBranch = branches[currentIndex - 1];
                _leftBranchImage.sprite = leftBranch.Sprite;
                _leftButton.gameObject.SetActive(true);
                _leftButton.SetInteractable(leftBranch.IsUnlocked);
                _leftBlock.SetActive(!leftBranch.IsUnlocked);
                if (leftBranch.IsUnlocked)
                    _leftButtonImage.color = Color.white;
                else
                    _leftButtonImage.color = _blockButtonColor;
            }
            else
                _leftButton.gameObject.SetActive(false);

            if (currentIndex < branches.Count - 1)
            {
                var rightBranch = branches[currentIndex + 1];
                _rightBranchImage.sprite = rightBranch.Sprite;
                _rightButton.gameObject.SetActive(true);
                _rightButton.SetInteractable(rightBranch.IsUnlocked);
                _rightBlock.SetActive(!rightBranch.IsUnlocked);
                if (rightBranch.IsUnlocked)
                    _rightButtonImage.color = Color.white;
                else
                    _rightButtonImage.color = _blockButtonColor;
            }
            else
                _rightButton.gameObject.SetActive(false);
        }
    }
}