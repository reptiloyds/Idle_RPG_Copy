using System;
using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Model;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.UI.StatImprove
{
    [Serializable]
    public class StatCardSetup
    {
        public UnitStatType Type;
        public bool RoundFirstGrade = true;
    }
    
    [DisallowMultipleComponent, HideMonoScript]
    public class StatImproverPage : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField, Required] private StatCard _cardTemplate;
        [SerializeField, Required] private Transform _cardParent;
        [SerializeField, Required] private TextMeshProUGUI _powerText;
        [SerializeField, Required] private List<StatCardSetup> _setups;
        [SerializeField] private bool _strippedCardColor = true;

        [Inject] private PowerCalculator _powerCalculator;
        [Inject] private StatImprover _statImprover;
        [Inject] private IObjectResolver _objectResolver;
        
        private List<StatCard> _statCards = new();
        private const string ButtonIdSeparator = "_";
        private IDisposable _disposable;

        void IInitializable.Initialize()
        {
            _disposable = _powerCalculator.Power
                .Skip(1)
                .Subscribe(SetPowerText);
            Setup();
        }

        void IDisposable.Dispose()
        {
            _disposable?.Dispose();
            foreach (var card in _statCards) 
                card.Purchase -= OnPurchase;
        }

        private void Setup()
        {
            var stats = _statImprover.Stats;
            if (stats.Count > _statCards.Count) 
                SpawnCards(stats.Count - _statCards.Count);

            for (int i = 0; i < _statCards.Count; i++)
            {
                var statCard = _statCards[i];
                if (i < stats.Count)
                {
                    var stat = stats[i];
                    StatCardSetup cardSetup = null;
                    foreach (var setup in _setups)
                    {
                        if (setup.Type != stat.Type) continue;
                        cardSetup = setup;
                        break;
                    }
                    
                    cardSetup ??= new StatCardSetup()
                    {
                        Type = stat.Type
                    };
                    _statImprover.UnlockConditions.TryGetValue(stat, out var unlock);
                    statCard.Setup(cardSetup, unlock);
                    SetupButtonId(statCard.Button, i + 1);
                    statCard.SetStat(stat);
                    statCard.Enable();
                }
                else
                {
                    statCard.ClearStat();
                    statCard.Disable();
                }
            }

            _statCards = _statCards.OrderBy(item => item.IsLevelMax).ToList();

            if (_strippedCardColor)
            {
                for (int i = 0; i < _statCards.Count; i++)
                {
                    var statCard = _statCards[i];
                    statCard.transform.SetSiblingIndex(i);
                    if (i % 2 == 0)
                        statCard.SetSecondColor();
                    else
                        statCard.SetFirstColor();
                }   
            }
        }

        private void SetupButtonId(BaseButton button, int number)
        {
            var id = button.Id;
            var split = id.Split(ButtonIdSeparator);
            button.ChangeButtonId($"{split[0]}_{number}");
        }

        public void Unlock(UnitStatType statType) => 
            GetCard(statType).Unlock();

        private void SetPowerText(BigDouble.Runtime.BigDouble power) => 
            _powerText.SetText(StringExtension.Instance.CutBigDouble(power, true));

        private void SpawnCards(int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                var card = _objectResolver.Instantiate(_cardTemplate, _cardParent);
                card.Initialize();
                card.Purchase += OnPurchase;
                _statCards.Add(card);
            }
        }

        private void OnPurchase(UnitStatType statType) => 
            _statImprover.Purchase(statType);

        private StatCard GetCard(UnitStatType statType)
        {
            foreach (var card in _statCards)
            {
                if(card.Type == statType)
                    return card;
            }

            return null;
        }
    }
}