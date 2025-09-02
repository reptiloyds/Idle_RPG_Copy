using PleasantlyGames.RPG.Runtime.AssetManagement.LoadUnits;
using PleasantlyGames.RPG.Runtime.Bootstrap.LoadUnits;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.NutakuRuntime.LoadUnits
{
    public class NutakuServicesLoadUnit : CompositeLoadUnit, IInitializable
    {
        [Inject] private NutakuLoadUnit _nutaku;
        [Inject] private ServerLoadUnit _server;
        [Inject] private NutakuTimeLoadUnit _nutakuTime;
        [Inject] private CheckVersionLoadUnit _checkVersion;
        [Inject] private BalanceLoadUnit _balance;
        [Inject] private ProgressDataLoadUnit _data;
        [Inject] private UpdateCatalogLoadUnit _updateCatalogLoadUnit;
        
        [Preserve]
        public NutakuServicesLoadUnit(ILoadUnit[] loadUnits, string description) : base(loadUnits, description)
        {
        }

        [Preserve]
        public NutakuServicesLoadUnit(string description) : base(description)
        {
        }

        public void Initialize()
        {
            Append(_balance);
            Append(_nutaku);
            Append(_server);
            Append(_checkVersion);
            Append(new ILoadUnit[]
            {
                _nutakuTime,
                _data,
                _updateCatalogLoadUnit,
            });
        }
    }
}