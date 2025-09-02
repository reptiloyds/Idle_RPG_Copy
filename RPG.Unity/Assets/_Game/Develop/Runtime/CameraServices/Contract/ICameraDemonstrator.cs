using System;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.CameraServices.Contract
{
    public interface ICameraDemonstrator
    {
        event Action OnComplete;
        void Demonstrate(Transform point);
    }
}