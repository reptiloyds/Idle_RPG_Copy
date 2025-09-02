using R3;
using Sirenix.OdinInspector;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Combat
{
    public class Targets : UnitComponent
    {
        [ShowInInspector, HideInEditorMode, ReadOnly] private UnitView _editorOnlyTarget;
        
        public ReactiveProperty<UnitView> Enemy { get; } = new();

        [Button]
        public void Set(UnitView unitView)
        {
            _editorOnlyTarget = unitView;
            Enemy.Value = unitView;
        }

        [Button]
        public void Clear()
        {
            _editorOnlyTarget = null;
            Enemy.Value = null;
        }
    }
}