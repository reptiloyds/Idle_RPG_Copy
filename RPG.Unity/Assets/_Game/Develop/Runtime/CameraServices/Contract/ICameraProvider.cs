using Cinemachine;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.CameraServices.Contract
{
    public interface ICameraProvider
    {
        Camera GetCamera();
        CinemachineBrain GetBrain();
    }
}