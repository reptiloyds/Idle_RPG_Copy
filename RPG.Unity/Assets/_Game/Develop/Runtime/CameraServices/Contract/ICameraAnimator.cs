using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.CameraServices.Defenition.Animation;

namespace PleasantlyGames.RPG.Runtime.CameraServices.Contract
{
    public interface ICameraAnimator
    {
        void Execute(List<CameraAnimationData> animations, bool playSequentially);
        void Revert(float duration = 0);
    }
}