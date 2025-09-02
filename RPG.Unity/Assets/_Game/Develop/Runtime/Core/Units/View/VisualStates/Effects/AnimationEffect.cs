using Animancer;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Animations;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.Effects
{
    public class AnimationEffect : BaseAnimancerEffect
    {
        [SerializeField] private AnimancerLayerType _layer;
        [SerializeField] private float _initWeight;
        [SerializeField] private ClipTransition[] _clips;
        [SerializeField] [HideIf("@this._clips.Length <= 1")]
        private bool _queueLaunch = true;
        [SerializeField] private SubStateType _nextSubState = SubStateType.None;
        
        protected int CurrentClipIndex = 0;
        protected ClipTransition[] Clips => _clips;

        public override void OnSpawn()
        {
            base.OnSpawn();
            
            foreach (var clip in _clips) 
                Controller.InitializeState(_layer, clip, _initWeight);
        }

        public override void Activate(UnitView unitView)
        {
            base.Activate(unitView);

            if (!_queueLaunch) 
                RandomizeIndex();
            
            var state = Play();
            state.Time = 0;
            if (!state.Clip.isLooping)
            {
                if (state.Events(this, out AnimancerEvent.Sequence events)) 
                    events.OnEnd = OnEnd;
            } 
            
            if (_clips.Length > 1 && _queueLaunch)
                IncrementIndex();
        }

        private void OnEnd()
        {
            if(_nextSubState != SubStateType.None)
                CurrentUnitView.StateMachine.SetSubState(_nextSubState);
        }

        private void IncrementIndex() => 
            CurrentClipIndex = (CurrentClipIndex + 1) % _clips.Length;

        private void RandomizeIndex() => 
            CurrentClipIndex = Random.Range(0, _clips.Length);

        protected virtual AnimancerState Play()
        {
            return Controller.PlayOnLayer(_layer, _clips[CurrentClipIndex]);
        }
    }
}