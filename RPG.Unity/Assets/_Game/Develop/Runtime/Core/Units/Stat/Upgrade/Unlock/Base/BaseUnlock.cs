using System;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;

namespace PleasantlyGames.RPG.Runtime.Core.Units.Stat.Upgrade.Unlock.Base
{
    public abstract class BaseUnlock : IUnlockable
    {
        protected ITranslator Translator;
        protected readonly UnitStat _stat;

        protected const string ConditionPrefix = "StatUnlockCondition_";

        public abstract bool IsUnlocked { get; }
        public UnitStat Stat => _stat;
        public event Action<BaseUnlock> OnUnlock;


        protected BaseUnlock(UnitStat stat, ITranslator translator)
        {
            Translator = translator;
            _stat = stat;
        }

        public virtual void Initialize(){}
        public virtual void Dispose(){}
        
        public abstract string Condition { get; }



        protected void TriggerOnUnlock() => 
            OnUnlock?.Invoke(this);

        public event Action<IUnlockable> OnUnlocked;
    }
}