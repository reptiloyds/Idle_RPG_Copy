using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Ad.Contracts;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Model.Effects;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Save;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Model;
using PleasantlyGames.RPG.Runtime.Core.GlobalStats.Type;
using PleasantlyGames.RPG.Runtime.Core.Stats.Extension;
using PleasantlyGames.RPG.Runtime.Core.Stats.Type;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Type;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Model
{
    public class BlessingService : IDisposable
    {
        [Inject] private BlessingDataProvider _dataProvider;
        [Inject] private BalanceContainer _balance;
        [Inject] private GlobalStatProvider _globalStatProvider;
        [Inject] private UnitStatService _unitStatService;
        [Inject] private IObjectResolver _resolver;
        [Inject] private ITranslator _translator;

        private BlessingSheet _blessingSheet;
        private GlobalBlessingSheet _globalBlessingSheet;

        private readonly List<Blessing> _blessings = new();
        private readonly List<GlobalBlessing> _globalBlessings = new();
        
        public IReadOnlyList<Blessing> Blessings => _blessings;
        public IReadOnlyList<GlobalBlessing> GlobalBlessings => _globalBlessings;

        public event Action<Blessing> OnBlessingEnabled;
        public event Action<Blessing> OnBlessingDisabled;
        public event Action<BaseBlessing> OnBlessingLevelUpped; 
        
        [Preserve]
        public BlessingService()
        {
        }

        public void Initialize()
        {
            _blessingSheet = _balance.Get<BlessingSheet>();
            _globalBlessingSheet = _balance.Get<GlobalBlessingSheet>();

            CreateBlessings();
            CreateGlobalBlessings();
        }

        public void ActivateBlessing(Blessing blessing)
        {
            blessing.Enable();
            blessing.AppendProgression();
        }

        private void CreateBlessings()
        {
            var dataContainer = _dataProvider.GetData();
            foreach (var data in dataContainer.List)
            {
                var config = _blessingSheet[data.Id];
                var effects = new List<BlessingEffect>();
                foreach (var effectConfig in config)
                    effects.Add(CreateEffect(effectConfig, data.Id, GroupOrder.Blessing));
                var blessing = new Blessing(config, data, effects);
                _resolver.Inject(blessing);
                _blessings.Add(blessing);
                blessing.OnLevelUpped += OnLevelUpped;
                blessing.OnEnabled += OnBlessingEnable;
                blessing.OnDisabled += OnBlessingDisable;
                blessing.Initialize();
            }
        }

        private void OnLevelUpped(BaseBlessing blessing) => 
            OnBlessingLevelUpped?.Invoke(blessing);

        private void CreateGlobalBlessings()
        {
            var dataContainer = _dataProvider.GetData();
            foreach (var data in dataContainer.GlobalList)
            {
                var config = _globalBlessingSheet[data.Id];
                var effects = new List<BlessingEffect>();
                foreach (var effectConfig in config)
                    effects.Add(CreateEffect(effectConfig, data.Id, GroupOrder.GlobalBlessing));
                var blessing = new GlobalBlessing(config, data, effects);
                _resolver.Inject(blessing);
                _globalBlessings.Add(blessing);
                blessing.Initialize();
            }
        }

        private void OnBlessingEnable(Blessing blessing) => 
            OnBlessingEnabled?.Invoke(blessing);

        private void OnBlessingDisable(Blessing blessing) => 
            OnBlessingDisabled?.Invoke(blessing);

        private BlessingEffect CreateEffect(BlessingElem blessingElem, string blessingId, GroupOrder groupOrder)
        {
            BlessingEffect result;
            switch (blessingElem.StatType)
            {
                case StatType.Unit:
                    var unitType = blessingElem.GetEffectType<UnitStatType>();
                    var blessingStat = _unitStatService.GetPlayerStat(unitType);

                    var baseValueFormula = blessingElem.EffectFormulaType.CreateFormula(blessingElem.EffectFormulaJSON);
                    var unitStatName = _translator.Translate(unitType.ToString());
                    result = new UnitStatBlessingEffect(blessingElem.EffectModType, baseValueFormula, blessingStat, unitStatName, blessingElem.ZeroOnFirstLevel, groupOrder);
                    break;
                case StatType.Global:
                    var globalType = blessingElem.GetEffectType<GlobalStatType>();
                    var globalStat = _globalStatProvider.GetStat(globalType);
                    
                    var globalValueFormula = blessingElem.EffectFormulaType.CreateFormula(blessingElem.EffectFormulaJSON);
                    var globalStatName = _translator.Translate(globalType.ToString());
                    result = new GlobalStatBlessingEffect(blessingElem.EffectModType, globalValueFormula, globalStat, globalStatName, blessingElem.ZeroOnFirstLevel, groupOrder);
                    break;
                default:
                    throw new Exception($"BlessingEffect type of {blessingElem.StatType} not implemented");
            }

            result.SetDebugName($"blessing_{blessingId}");
            return result;
        }

        public void Dispose()
        {
            foreach (var blessing in _blessings)
            {
                blessing.OnLevelUpped -= OnLevelUpped;
                blessing.OnEnabled -= OnBlessingEnable;
                blessing.OnDisabled -= OnBlessingDisable;
                blessing.Dispose(); 
            } 
            
            foreach (var blessing in _globalBlessings) 
                blessing.Dispose();
        }
    }
}