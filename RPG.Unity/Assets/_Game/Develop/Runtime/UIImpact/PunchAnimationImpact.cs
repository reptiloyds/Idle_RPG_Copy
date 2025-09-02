using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.UIImpact
{
    public class PunchAnimationImpact : MonoBehaviour
    {
        [SerializeField] private List<RectTransform> _targets;
        [SerializeField] private ShakeSettings _settings;

        public void DoPunch()
        {
            foreach (var target in _targets)
                Tween.PunchScale(target, _settings);
        }
    }
}