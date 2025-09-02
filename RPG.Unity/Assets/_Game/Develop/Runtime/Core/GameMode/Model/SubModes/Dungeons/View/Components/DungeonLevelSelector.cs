using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View.Components
{
    [DisallowMultipleComponent, HideMonoScript]
    public class DungeonLevelSelector : MonoBehaviour
    {
        [SerializeField] private BaseButton _leftButton;
        [SerializeField] private BaseButton _rightButton;
        [SerializeField] private TextMeshProUGUI _currentLevelText;
        [SerializeField] private GameObject _availableLevelObject;
        [SerializeField] private GameObject _previousLevelObject;
        [SerializeField] private List<Graphic> _colorizedTexts;
        [SerializeField] private Color _availableColor;
        [SerializeField] private Color _previousColor;

        private int _currentLevel;
        private int _availableLevel;
        private int _maxLevel;

        public bool IsLevelAvailable => _currentLevel == _availableLevel;
        public int CurrentLevel => _currentLevel;
        public event Action OnCurrentLevelChanged;

        private void Awake()
        {
            _leftButton.OnClick += OnLeftButtonClick;
            _rightButton.OnClick += OnRightButtonClick;
        }

        private void OnDestroy()
        {
            _leftButton.OnClick -= OnLeftButtonClick;
            _rightButton.OnClick -= OnRightButtonClick;
        }

        private void OnLeftButtonClick()
        {
            _currentLevel--;
            Redraw();
            OnCurrentLevelChanged?.Invoke();
        }

        private void OnRightButtonClick()
        {
            _currentLevel++;
            Redraw();
            OnCurrentLevelChanged?.Invoke();
        }

        public void Setup(int availableLevel, int maxLevel)
        {
            _availableLevel = availableLevel;
            _currentLevel = _availableLevel;
            _maxLevel = maxLevel;
            Redraw();
        }

        private void Redraw()
        {
            _currentLevelText.SetText(_currentLevel.ToString());

            var isLevelAvailable = IsLevelAvailable;
            _leftButton.gameObject.SetActive(_currentLevel != 1);
            _rightButton.gameObject.SetActive(_currentLevel != _maxLevel);
            _rightButton.SetInteractable(!isLevelAvailable);
            _availableLevelObject.SetActive(isLevelAvailable);
            _previousLevelObject.SetActive(!isLevelAvailable);

            foreach (var graphic in _colorizedTexts) 
                graphic.color = isLevelAvailable ? _availableColor : _previousColor;
        }
    }
}