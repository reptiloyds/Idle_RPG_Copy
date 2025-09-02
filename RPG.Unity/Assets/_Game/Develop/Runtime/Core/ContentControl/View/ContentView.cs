using System;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.ContentControl.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class ContentView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Image _image;
        [SerializeField] private BaseButton _receiveButton;
        [SerializeField] private Slider _progressBar;
        [SerializeField] private GameObject _blockObject;
        [SerializeField] private TextMeshProUGUI _unlockCondition;

        public Content Content { get; private set; }
        
        public event Action<ContentView> OnReceiveClicked;

        private void Awake() => 
            _receiveButton.OnClick += OnReceiveClick;

        private void OnDestroy()
        {
            _receiveButton.OnClick -= OnReceiveClick;
            if (Content != null)
            {
                Content.OnProgressChanged -= RedrawDynamicData;
                Content.OnReadyToManualUnlock -= RedrawDynamicData;
            } 
        }

        private void OnReceiveClick() => 
            OnReceiveClicked?.Invoke(this);

        public void Setup(Content content)
        {
            if(Content != null) return;
            Content = content;
            _receiveButton.ChangeButtonId($"Content_{content.Id}");
            Content.OnProgressChanged += RedrawDynamicData;
            Content.OnReadyToManualUnlock += RedrawDynamicData;
            RedrawStaticData();
            RedrawDynamicData();
        }

        private void RedrawStaticData()
        {
            _name.SetText(Content.Name);
            _image.sprite = Content.Image;
            _unlockCondition.SetText(Content.Condition);
        }

        private void RedrawDynamicData()
        {
            _blockObject.SetActive(!Content.IsReadyForManualUnlock);
            _receiveButton.SetInteractable(Content.IsReadyForManualUnlock);
            _progressBar.value = Content.Progress;
        }
    }
}