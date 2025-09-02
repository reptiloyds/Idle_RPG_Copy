using System;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Utilities.ThirdParty.Model
{
    public class ThirdPartyEvents
    {
        public event Action<bool> OnInitializationFailed;

        [Preserve]
        public ThirdPartyEvents()
        {
            
        }
        
        public void InitializationFailedInvoke(bool handle = true) => 
            OnInitializationFailed?.Invoke(handle);
    }
}