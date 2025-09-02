using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.CameraServices.Contract
{
    public interface ICameraChaser
    {
        void Follow(Transform target);
        void LookAt(Transform target);
    }
}