using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base
{
    [HideMonoScript, DisallowMultipleComponent]
    public class BaseButtonSwitcher : MonoBehaviour
    {
        [SerializeField, Required] private BaseButton _leftSwitch;
        [SerializeField, Required] private BaseButton _leftButton;
        
        [SerializeField, Required] private BaseButton _rightSwitch;
        [SerializeField, Required] private BaseButton _rightButton;

        protected virtual void Awake()
        {
            _leftSwitch.OnClick += OnLeftClick;
            _rightSwitch.OnClick += OnRightClick;
        }

        protected virtual void OnDestroy()
        {
            _leftSwitch.OnClick -= OnLeftClick;
            _rightSwitch.OnClick -= OnRightClick;
        }

        private void OnLeftClick() => ShowLeft();

        private void OnRightClick() => ShowRight();

        protected void ShowLeft()
        {
            _leftSwitch.gameObject.SetActive(false);
            _leftButton.gameObject.SetActive(true);
            _rightSwitch.gameObject.SetActive(true);
            _rightButton.gameObject.SetActive(false);
        }

        protected void ShowRight()
        {
            _leftSwitch.gameObject.SetActive(true);
            _leftButton.gameObject.SetActive(false);
            _rightSwitch.gameObject.SetActive(false);
            _rightButton.gameObject.SetActive(true);
        }

        protected void ShowOnlyLeft()
        {
            _leftSwitch.gameObject.SetActive(false);
            _leftButton.gameObject.SetActive(true);
            _rightSwitch.gameObject.SetActive(false);
            _rightButton.gameObject.SetActive(false);
        }

        protected void ShowOnlyRight()
        {
            _leftSwitch.gameObject.SetActive(false);
            _leftButton.gameObject.SetActive(false);
            _rightSwitch.gameObject.SetActive(false);
            _rightButton.gameObject.SetActive(true);
        }
    }
}