using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Movement
{
    public class Movement : BaseMovement
    {
        [SerializeField, Required] private NavMeshAgent _agent;
        [SerializeField] private bool _stopInstantly = false;

        public override float Radius => _agent.radius;
        public override float Height => _agent.height;

        protected override void GetComponents()
        {
            base.GetComponents();
            _agent ??= GetComponent<NavMeshAgent>();
        }
        
        public override void MoveTo(Vector3 position, float offset = 0)
        {
            base.MoveTo(position, offset);

            if (_stopInstantly) _agent.updatePosition = true;
            _agent.updateRotation = true;
            _agent.isStopped = false;
            _agent.SetDestination(DestinationPoint);
        }
        
        public override void Stop()
        {
            StopAgent();
            base.Stop();
        }

        public override void Dispose()
        {
            base.Dispose();
            
            StopAgent();
        }

        private void StopAgent()
        {
            if (_stopInstantly) _agent.updatePosition = false;
            if(!_agent.isActiveAndEnabled || !_agent.isOnNavMesh) return;
            _agent.isStopped = true;
            _agent.updateRotation = false;
        }

        public override void SetPosition(Vector3 position) => 
            _agent.Warp(position);

        public override void SetRotation(Vector3 direction) =>
            transform.rotation = Quaternion.LookRotation(direction);

        protected override void OnUpdateSpeed() => 
            _agent.speed = (float)SpeedStat.Value.ToDouble();
        
        protected override bool IsDistanceReached() => 
            !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance && (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f);
    }
}