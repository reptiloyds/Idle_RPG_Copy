using PleasantlyGames.RPG.Runtime.CameraServices.Contract;
using PleasantlyGames.RPG.Runtime.CameraServices.Model.Mediator;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using CameraType = PleasantlyGames.RPG.Runtime.CameraServices.Type.CameraType;

namespace PleasantlyGames.RPG.Runtime.CameraServices.Model
{
    public class CameraChaser : ICameraChaser, IInitializable
    {
        private CinemachineInterlayer _camera;
        [Inject] private IVirtualCameraService _virtualCameraService;

        public void Initialize() => 
            _camera = _virtualCameraService.GetCamera(CameraType.Main);

        void ICameraChaser.Follow(Transform target) => 
            _camera.Follow(target);

        void ICameraChaser.LookAt(Transform target) => 
            _camera.LookAt(target);
    }
}
