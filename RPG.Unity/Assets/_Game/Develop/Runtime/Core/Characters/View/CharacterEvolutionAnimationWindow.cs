using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CharacterEvolutionAnimationWindow : BaseWindow
    {
        [SerializeField] private Image _image;
        [SerializeField, MinValue(0)] private float _animationDuration;

        public float AnimationDuration => _animationDuration;

        public void Setup(Character character) => 
            _image.sprite = character.Sprite;
    }
}