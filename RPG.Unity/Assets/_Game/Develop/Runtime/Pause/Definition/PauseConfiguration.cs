using System;
using PleasantlyGames.RPG.Runtime.Pause.Type;

namespace PleasantlyGames.RPG.Runtime.Pause.Definition
{
    [Serializable]
    public class PauseConfiguration
    {
        public PauseType ImportantAnimation = PauseType.Ad;
        public PauseType OpenWindow = PauseType.Input;
        public PauseType CameraMovement = PauseType.Input ^ PauseType.Ad ^ PauseType.UIInput;
        public PauseType BeforeAd = PauseType.Input ^ PauseType.Time ^ PauseType.Ad;
        public PauseType Ad = PauseType.Input ^ PauseType.Audio ^ PauseType.Time ^ PauseType.Ad;
    }
}