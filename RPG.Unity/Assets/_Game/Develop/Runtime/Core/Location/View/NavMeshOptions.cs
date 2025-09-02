using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace PleasantlyGames.RPG.Runtime.Core.Location.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class NavMeshOptions : MonoBehaviour
    {
        [SerializeField, MinValue(0)] private float _avoidancePredictionTime = 2;

        private void Awake() => 
            NavMesh.avoidancePredictionTime = _avoidancePredictionTime;
    }
}
