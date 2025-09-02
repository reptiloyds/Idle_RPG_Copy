using System;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.View;
using PleasantlyGames.RPG.Runtime.Core.Items.Companion.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Skill.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class SelectedSkillView : MonoBehaviour
    {
        [SerializeField, Required] private ItemView _itemView;
        [SerializeField, Required] private BaseButton _cancelButton;

        private CompanionItem _item;

        public event Action OnCancel;
        public CompanionItem Item => _item;
        public bool IsEnabled { get; private set; }

        private void Awake() => _cancelButton.OnClick += OnCancelClick;

        private void OnDestroy() => _cancelButton.OnClick -= OnCancelClick;

        private void OnCancelClick() => 
            OnCancel?.Invoke();

        public void Setup(CompanionItem item)
        {
            _item = item;
            _itemView.SetModel(_item);
            _itemView.Unselect();
            _itemView.HideEquippedObject();
        }

        private void Clear()
        {
            if (_item == null) return;
            _itemView.ClearModel();
            _item = null;
        }

        public void Enable()
        {
            gameObject.SetActive(true);
            IsEnabled = true;
        }

        public void Disable()
        {
            Clear();
            gameObject.SetActive(false);
            IsEnabled = false;
        }
    }
}