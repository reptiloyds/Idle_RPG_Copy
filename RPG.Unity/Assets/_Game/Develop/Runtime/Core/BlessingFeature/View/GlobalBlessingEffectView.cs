using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class GlobalBlessingEffectView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _effect;

        public void Setup(string text) => 
            _effect.SetText(text);
    }
}
