using System;
using PleasantlyGames.RPG.Runtime.Balance.Type;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace PleasantlyGames.RPG.Runtime.Balance.Definition
{
    [Serializable]
    public class BalanceConfiguration
    {
        public BalanceSource Source;
        [HideIf("@this.Source != BalanceSource.CDN")]
        public string CDNRoot;
        [HideIf("@this.Source != BalanceSource.CDN")]
        public bool UseCDNOnlyInBuild;
    }
}