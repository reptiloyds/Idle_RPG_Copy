using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Core.AudioTypes;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Definition;
using PleasantlyGames.RPG.Runtime.Core.Items.Base.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Items.Base.View.Enhance
{
    [DisallowMultipleComponent, HideMonoScript]
    public class EnhanceItemWindow : BaseWindow
    {
        [SerializeField, Required] private EnhanceItemView _itemPrefab;
        [SerializeField, Required] private Transform _itemContainer;
        [SerializeField, Required] private TextMeshProUGUI _previousOwnedModifier;
        [SerializeField, Required] private TextMeshProUGUI _currentOwnedModifier;
        [SerializeField, MinValue(0)] private float _spawnDelay = 0.2f;

        [Inject] private ItemConfiguration _itemConfiguration;
        [Inject] private ITranslator _translator;
        [Inject] private IAudioService _audioService;

        private readonly List<EnhanceItemView> _views = new();
        private readonly List<Item> _items = new();
        private readonly List<Tween> _tweens = new();

        public void AddItem(Item item) =>
            _items.Add(item);

        public override void Open()
        {
            Draw();
            base.Open();
        }

        public override void Close()
        {
            _items.Clear();
            foreach (var tween in _tweens) 
                tween.Stop();
            foreach (var view in _views)
                view.Clear();

            base.Close();
        }

        private void Draw()
        {
            DrawPreviousModifier();
            
            _tweens.Clear();
            
            var viewId = 0;
            foreach (var item in _items)
            {
                var view = GetView(viewId);
                view.Disable();
                view.SetModel(item);
                item.Enhance();
                view.DrawCurrentLevel();
                float delay = viewId * _spawnDelay;
                if (delay > 0)
                {
                    var tween = Tween.Delay(delay, () => view.Enable());
                    _tweens.Add(tween);
                }
                else
                    view.Enable();
                viewId++;
            }
            
            _audioService.CreateLocalSound(UI_Effect.UI_ItemUpgrade).Play();

            for (; viewId < _views.Count; viewId++) 
                _views[viewId].Disable();
            
            DrawCurrentModifier();
        }

        private EnhanceItemView GetView(int id)
        {
            if (id >= _views.Count)
            {
                var item = Instantiate(_itemPrefab, _itemContainer);
                _views.Add(item);
                return item;
            }

            return _views[id];
        }

        private void DrawPreviousModifier()
        {
            var ownedSum = GetOwnedModifierSum();
            _previousOwnedModifier.SetText($"{_translator.Translate(ownedSum.Type.ToString())}: +{StringExtension.Instance.CutBigDouble(ownedSum.Value)}%");
        }

        private void DrawCurrentModifier()
        {
            var ownedSum = GetOwnedModifierSum();
            _currentOwnedModifier.SetText($"+{StringExtension.Instance.CutBigDouble(ownedSum.Value)}%");
        }
        
        private (UnitStatType Type, BigDouble.Runtime.BigDouble Value) GetOwnedModifierSum()
        {
            BigDouble.Runtime.BigDouble value = 0;
            UnitStatType type = UnitStatType.None;
            foreach (var item in _items)
            {
                if(!item.IsUnlocked) continue;
                value += item.OwnedModifier.Value;
                type = item.OwnedEffectType;
            }
            
            return (type, value * 100);
        }
    }
}