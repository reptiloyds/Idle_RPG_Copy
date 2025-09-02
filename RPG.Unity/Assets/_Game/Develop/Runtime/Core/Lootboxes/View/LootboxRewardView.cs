using System;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using PleasantlyGames.RPG.Runtime.Audio.Model;
using PleasantlyGames.RPG.Runtime.Core.GlowUIFeature;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Type;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.View;
using PleasantlyGames.RPG.Runtime.Core.TooltipsFeature.Factory;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.Tween;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Lootboxes.View
{
    public class LootboxRewardView : MonoBehaviour
    {
        [SerializeField, Required] private BaseButton _tooltipButton;
        [SerializeField, Required] private SlotEquippedImage _slotEquippedImage;
        [SerializeField, Required] private Image _image;
        [SerializeField, Required] private Image _backgroundImage;
        [SerializeField, Required] private UIScale _scaleAnimation;
        [SerializeField, Required] private List<ParticleImage> _colorfulParticles;
        [SerializeField, Required] private List<ParticleImage> _particles;
        [SerializeField, Range(0, 1)] private float _particleWhiteK = 0.3f;
        [SerializeField, Required] private GlowContainer _glow;

        [Inject] private TooltipFactory _tooltipFactory;
        
        private AudioEmitter _audioEmitter;
        private Tween _tween;
        private ItemRarityType _rarity;
        private string _itemId;
        private ItemType _type;

        public ItemRarityType Rarity => _rarity;
        public string ItemId => _itemId;

        private void Start()
        {
            _tooltipButton.OnClick += ShowTooltip;
        }

        private void OnDestroy()
        {
            _tooltipButton.OnClick -= ShowTooltip;
        }

        private void ShowTooltip()
        {
            _tooltipFactory.ShowItemTooltip(_type);
        }

        public void Setup(Sprite itemSprite, Color backgroundColor, ItemRarityType rarity, string itemId, ItemType type)
        {
            _type = type;
            _itemId = itemId;
            _rarity = rarity;
            _tween.Stop();
            _scaleAnimation.Stop();
            foreach (var particle in _particles) 
                particle.Stop(); 
            
            _image.sprite = itemSprite;
            _backgroundImage.color = backgroundColor;
            _scaleAnimation.Target.localScale = Vector3.zero;
            
            Color particleColor = Color.Lerp(backgroundColor, Color.white, _particleWhiteK);

            foreach (var colorfulParticle in _colorfulParticles)
            {
                var particleAlpha = colorfulParticle.color.a;
                var gradient = new ParticleSystem.MinMaxGradient(new Color(particleColor.r, particleColor.g, particleColor.b, particleAlpha));
                colorfulParticle.startColor = gradient;
            }
            
            switch (_rarity)
            {
                case ItemRarityType.Epic: 
                case ItemRarityType.Legendary: 
                case ItemRarityType.Mythic: 
                case ItemRarityType.Exotic:
                    _glow.SetColor(backgroundColor);
                    _glow.Show();
                    break;
                default:
                    _glow.Hide();
                    break;
            }
            
        }

        public void ShowSlotImage(Sprite slotSprite, Color backgroundColor)
        {
            _slotEquippedImage.Redraw(slotSprite, backgroundColor);
            _slotEquippedImage.Enable();
        }

        public void HideSlotImage() => 
            _slotEquippedImage.Disable();

        public void Play(float delay, AudioEmitter audioEmitter)
        {
            _audioEmitter = audioEmitter;
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
            _audioEmitter.Play();
        }
    }
}