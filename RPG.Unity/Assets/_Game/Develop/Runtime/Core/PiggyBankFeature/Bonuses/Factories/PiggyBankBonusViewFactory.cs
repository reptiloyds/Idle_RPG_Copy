using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.View;
using PleasantlyGames.RPG.Runtime.Core.UI;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Bonuses.Factories
{
    public class PiggyBankBonusViewFactory
    {
        [Inject] private IAssetProvider _assetProvider;
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private UIFactory _uiFactory;
        
        private readonly List<PiggyBankBonusView> _actives = new List<PiggyBankBonusView>();
        private PiggyBankBonusView _prefab;

        [UnityEngine.Scripting.Preserve]
        public PiggyBankBonusViewFactory()
        {
        }
        
        public async UniTask WarmUpAsync()
        {
            var result = await _uiFactory.LoadAsync(Asset.UI.PiggyBankBonusView, false);
            
            if (result.TryGetComponent(out PiggyBankBonusView view))
                _prefab = view;
            else
                Logger.LogError("No 'Bonus View' found during warm up.");
        }
        
        public PiggyBankBonusView Create()
        {
            if (_prefab == null)
                Logger.LogError("No 'Bonus View' prefab found during creation.");
            
            var productView = _objectResolver.Instantiate(_prefab);
            _actives.Add(productView);
            return productView;
        }
        
        public void Release(PiggyBankBonusView view)
        {
            if (view == null)
                return;
            
            view.transform.SetParent(null);
            _actives.Remove(view);
            Object.Destroy(view.gameObject);
        }
    }
}