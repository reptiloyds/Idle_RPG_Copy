using Cinemachine;
using PleasantlyGames.RPG.Runtime.CameraServices.Contract;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.CameraServices.Model
{
    [HideMonoScript, DisallowMultipleComponent]
    public class CameraProvider : MonoBehaviour, ICameraProvider
    {
        [SerializeField, Required] private Camera _mainCamera;
        [SerializeField, Required] private CinemachineBrain _brain;

        public Camera GetCamera() => 
            _mainCamera;

        public CinemachineBrain GetBrain() => 
            _brain;
    }
}