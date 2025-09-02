using System;
using PleasantlyGames.RPG.Runtime.CameraServices.Defenition;
using PleasantlyGames.RPG.Runtime.CameraServices.Model.Mediator;
using PleasantlyGames.RPG.Runtime.CameraServices.Type;

namespace PleasantlyGames.RPG.Runtime.CameraServices.Contract
{
    public interface IVirtualCameraService
    {
        event Action OnCameraChanged;
        CameraType GetCurrentCameraType();
        CinemachineInterlayer GetCurrentCamera();
        CinemachineInterlayer GetCamera(CameraType type);
        void SwitchTo(CameraType type, float blendTime = -1f);
        void SwitchTo(VirtualCameraConfig cameraConfig, float blendTime = -1f);
    }
}