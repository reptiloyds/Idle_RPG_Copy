using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Ad.Contracts
    ;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Model.Effects;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Save;
using PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.VIP.Model;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Model
{
    public class GlobalBlessing : BaseBlessing
    {
        [Inject] private VipService _vipService;
        private readonly GlobalBlessingSheet.Row _config;

        public GlobalBlessing(GlobalBlessingSheet.Row config, BlessingData data, List<BlessingEffect> effects) :
            base(config, data, effects, config.MaxLevel) => 
            _config = config;

        public override void Initialize()
        {
            base.Initialize();

            _vipService.OnActivate += OnVipActivated;
            Enable();
        }

        public override void Dispose()
        {
            base.Dispose();
            _vipService.OnActivate -= OnVipActivated;
        }

        private void OnVipActivated() => 
            AppendProgression();

        protected override BaseValueFormula GetLevelProgression() => 
            _config.LevelFormula.CreateFormula(_config.LevelFormulaJSON);
    }
}