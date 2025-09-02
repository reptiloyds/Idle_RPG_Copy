using PleasantlyGames.RPG.Runtime.Bootstrap.LoadUnits;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using PleasantlyGames.RPG.YGRuntime.TimeUtility.LoadUnit;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.YGRuntime.LoadUnits
{
    public class WebServicesLoadUnit : CompositeLoadUnit, IInitializable
    {
        [Inject] private GamePushInAppLoadUnit _inAppLoadUnit;
        [Inject] private GamePushServerTimeLoadUnit _timeLoadUnit;
        
        public WebServicesLoadUnit(ILoadUnit[] loadUnits, string description) : base(loadUnits, description)
        {
        }

        public WebServicesLoadUnit(string description) : base(description)
        {
        }

        public void Initialize()
        {
            Append(new ILoadUnit[]
            {
                _timeLoadUnit,
                _inAppLoadUnit,
            });
        }
    }
}