using PleasantlyGames.RPG.Runtime.Core.Units.View.Building;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View
{
    [HideMonoScript]
    public abstract class UnitComponent : MonoBehaviour, IUpdateableBuildElement
    {
        [SerializeField, Required] protected UnitView Unit;

        protected virtual void Reset() => GetComponents();

        public virtual void OnValidate() => GetComponents();

        protected virtual void GetComponents() => Unit ??= GetComponentInParent<UnitView>();

        public virtual void Initialize() { }

        public virtual void OnSpawn() { }

        public virtual void Dispose() { }

        void IBuildElement.LogIfWrong(ref int errorCount) { }

        void IUpdateableBuildElement.UpdateState(UnitView unitView) => 
            GetComponents();
    }
}