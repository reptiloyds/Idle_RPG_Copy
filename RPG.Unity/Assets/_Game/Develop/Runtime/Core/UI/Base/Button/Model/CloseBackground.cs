using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model
{
    [HideMonoScript]
    [DisallowMultipleComponent]
    public class CloseBackground : MonoBehaviour
    {
        [SerializeField, Required] private BaseButton _baseButton;
        [SerializeField, Required] private BaseWindow _window;

        private void Reset()
        {
            _baseButton ??= GetComponent<BaseButton>();
        }

        private void Awake()
        {
            _baseButton.OnClick += OnClick;
        }

        private void OnDestroy()
        {
            _baseButton.OnClick -= OnClick;
        }

        private void OnClick()
        {
            _window.Close();
        }
    }
}
