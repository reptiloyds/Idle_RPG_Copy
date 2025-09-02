using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Base.Text
{
    [DisallowMultipleComponent, HideMonoScript]
    public class AutoSizeFont : MonoBehaviour
    {
        [Serializable]
        private class TextSizeData
        {
            public Vector2 CharacterRange;
            public float FontSize;
        }
        
        [SerializeField, Required] private TextMeshProUGUI _text;
        [SerializeField] private List<TextSizeData> _sizeDataList;

        private bool _initialized;
        private int _lastCharacterCount;
        private TextSizeData _lastSizeData;

        private void Reset() => _text = GetComponent<TextMeshProUGUI>();

        private void Awake()
        {
            if(_text.enableAutoSizing) return;

            _initialized = true;
            _text.OnPreRenderText += OnPreRenderText;
        }

        private void Start() => 
            OnPreRenderText(_text.textInfo);

        private void OnDestroy()
        {
            if(_text != null && _initialized)
                _text.OnPreRenderText -= OnPreRenderText;
        }

        private void OnPreRenderText(TMP_TextInfo textInfo)
        {
            var characterCount = _text.text.Length;
            if(_lastCharacterCount == characterCount) return;
            
            foreach (var sizeData in _sizeDataList)
            {
                if(sizeData.CharacterRange.x <= characterCount && sizeData.CharacterRange.y >= characterCount)
                    ChangeFontSize(sizeData);
            }

            _lastCharacterCount = characterCount;
        }

        private void ChangeFontSize(TextSizeData sizeData)
        {
            if(_lastSizeData == sizeData) return;

            _lastSizeData = sizeData;
            _text.fontSize = _lastSizeData.FontSize;
        }
    }
}