using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View.Components
{
    [DisallowMultipleComponent, HideMonoScript]
    public class DungeonAutoSweepView : MonoBehaviour
    {
        [SerializeField] private Toggle _toggle;

        public bool IsOn => _toggle.isOn;
        public event Action OnToggled;

        private void Awake() => 
            _toggle.onValueChanged.AddListener(OnToggle);

        private void OnDestroy() => 
            _toggle.onValueChanged.RemoveAllListeners();

        private void OnToggle(bool value) => 
            OnToggled?.Invoke();

        public void SetValue(bool value) => 
            _toggle.isOn = value;
    }
}