using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Chats.View
{
    public class ChatAnswerButton : BaseButton
    {
        [SerializeField] private TextMeshProUGUI _message;

        [ShowInInspector, HideInEditorMode, ReadOnly]
        private string _variantKey;

        [Inject] private ITranslator _translator;
        
        public event Action<string> OnSelected;

        public void SetVariant(string variantKey)
        {
            _variantKey = variantKey;
            _message.SetText(_translator.Translate("chatA_" + variantKey));
        }

        protected override void Click()
        {
            base.Click();
            
            OnSelected?.Invoke(_variantKey);
        }
    }
}