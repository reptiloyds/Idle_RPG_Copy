using System.Collections.Generic;
using AssetKits.ParticleImage;
using PleasantlyGames.RPG.Runtime.Core.UI.Tween;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View.Components
{
    [DisallowMultipleComponent, HideMonoScript]
    public class DungeonRewardCard : MonoBehaviour
    {
        [SerializeField, Required] private Image _image;
        [SerializeField, Required] private Image _backgroundImage;
        [SerializeField, Required] private TextMeshProUGUI _amountText;
        [SerializeField, Required] private UIScale _scaleAnimation;
        [SerializeField, Required] private List<ParticleImage> _particles;

        private Tween _tween;
        
        public void Setup(Sprite sprite, string amount, Color backColor)
        {
            _tween.Stop();
            _scaleAnimation.Stop();
            foreach (var particle in _particles) 
                particle.Stop(); 
            
            _image.sprite = sprite;
            _backgroundImage.color = backColor;
            _amountText.SetText($"x{amount}");
            _scaleAnimation.Target.localScale = Vector3.zero;
        }
        
        public void Play(float delay = 0)
        {
            _tween.Stop();
            if (delay > 0)
                _tween = Tween.Delay(delay, StartAnimations, useUnscaledTime: true);
            else
                StartAnimations();
        }

        private void StartAnimations()
        {
            _scaleAnimation.Play();
            foreach (var particle in _particles) 
                particle.Play();
        }
    }
}