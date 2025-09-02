using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base
{
    public enum DealType
    {
        OneStep = 0,
        CompleteAllSteps = 1,
    }
    
    [HideMonoScript]
    public abstract class BaseDealButton : BaseButton
    {
        [SerializeField] private DealType _dealType = DealType.OneStep;
        
        protected bool IsAutoUpdatable = true;
        
        public event Action<DealType, bool> OnDeal;
        
        protected void CompleteDeal() => OnDeal?.Invoke(_dealType, true);
        
        protected void FailDeal() => OnDeal?.Invoke(_dealType, false);

        public void SetDealType(DealType dealType) => _dealType = dealType;

        public void BlockAutoUpdateInteraction()
        {
            IsAutoUpdatable = false;
        }

        public void UnlockAutoUpdateInteraction()
        {
            IsAutoUpdatable = true;
            AutoUpdateInteraction();
        }

        protected virtual void AutoUpdateInteraction()
        {
        }
    }
}