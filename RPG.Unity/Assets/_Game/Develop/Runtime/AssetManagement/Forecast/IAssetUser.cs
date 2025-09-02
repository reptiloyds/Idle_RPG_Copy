using System;
using System.Collections.Generic;

namespace PleasantlyGames.RPG.Runtime.AssetManagement.Forecast
{
    public interface IAssetUser
    {
        event Action OnNeedsChanged;
        void FillNeeds(in Dictionary<AssetType, HashSet<string>> needs);
    }
}