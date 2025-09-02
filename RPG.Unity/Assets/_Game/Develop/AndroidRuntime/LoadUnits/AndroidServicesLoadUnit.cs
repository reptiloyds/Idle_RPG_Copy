using PleasantlyGames.RPG.AndroidRuntime.Firebase;
using PleasantlyGames.RPG.Runtime.Bootstrap.LoadUnits;
using PleasantlyGames.RPG.Runtime.PrivacyPolicy.LoadUnit;
using PleasantlyGames.RPG.Runtime.TimeUtilities.LoadUnits;
using PleasantlyGames.RPG.Runtime.Utilities.Loading;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.AndroidRuntime.LoadUnits
{
    public class AndroidServicesLoadUnit : CompositeLoadUnit, IInitializable
    {
        [Inject] private PrivacyPolicyLoadUnit _privacyPolicy;
        [Inject] private AdMobLoadUnit _adMob;
        [Inject] private BalanceLoadUnit _balance;
        [Inject] private UnityPurchaseLoadUnit _unityPurchase;
        [Inject] private WebRequestTimeLoadUnit _time;
        [Inject] private ProgressDataLoadUnit _data;
        [Inject] private FirebaseLoadUnit _firebase;

        public AndroidServicesLoadUnit(ILoadUnit[] loadUnits, string description) : base(loadUnits, description)
        {
        }

        public AndroidServicesLoadUnit(string description) : base(description)
        {
        }

        public void Initialize()
        {
            Append(_privacyPolicy);
            Append(new ILoadUnit[]
            {
                _adMob,
                _balance,
                _time,
                _data,
                _firebase,
            });
            Append(_unityPurchase);
        }
    }
}