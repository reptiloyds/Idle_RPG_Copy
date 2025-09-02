using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.UnityExtension;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.LoadingLabelSwitchFeature.View
{
    public class LoadingLabelTextSwitcher: MonoBehaviour
    {
        [SerializeField, Required] private TMP_Text _text;
        [SerializeField] private List<string> _tokens = new List<string>()
        {
            "loading_label_token_1",
            "loading_label_token_2",
            "loading_label_token_3",
            "loading_label_token_4",
            "loading_label_token_5",
            "loading_label_token_6",
            "loading_label_token_7",
            "loading_label_token_8",
            "loading_label_token_9",
            "loading_label_token_10",
            "loading_label_token_11",
            "loading_label_token_12",
            "loading_label_token_13",
            "loading_label_token_14",
            "loading_label_token_15",
            "loading_label_token_16",
            "loading_label_token_17",
            "loading_label_token_18",
            "loading_label_token_19",
            "loading_label_token_20",
        };
        [SerializeField] private float _labelTime;

        [Inject] private ITranslator _translator;

        private Tween _tween;

        private void Start()
        {
            UpdateLabel();
            StartTween();
        }

        private void OnDestroy()
        {
            if (_tween.isAlive)
                _tween.Stop();
        }
        
        private void UpdateLabel()
        {
            _text.text = GetRandomLabel();
        }

        private void StartTween()
        {
            _tween = Tween.Delay(_labelTime, () =>
            {
                UpdateLabel();
                StartTween();
            });
        }
        
        private string GetRandomLabel()
        {
            return _translator.Translate(_tokens.GetRandomElement());
        }
    }
}