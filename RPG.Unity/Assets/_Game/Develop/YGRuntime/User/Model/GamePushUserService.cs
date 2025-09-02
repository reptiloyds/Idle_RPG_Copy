using GamePush;
using PleasantlyGames.RPG.Runtime.User.Definition;
using PleasantlyGames.RPG.Runtime.User.Model;
using PleasantlyGames.RPG.Runtime.User.Type;
using VContainer;

namespace PleasantlyGames.RPG.YGRuntime.User.Model
{
    internal sealed class GamePushUserService : BaseUserService
    {
        [Inject] private DeviceSimulationConfig _configuration;
        
        private int _playerId;
        private bool _playerIdInitialized;

        [Preserve]
        public GamePushUserService()
        { }

        public override int GetId()
        {
            if (!_playerIdInitialized)
                InitializePlayerId();

            return _playerId;
        }

        private void InitializePlayerId()
        {
            _playerId = GP_Player.GetID();
            _playerIdInitialized = true;
        }

        protected override bool IsMobileDevice()
        {
#if UNITY_EDITOR
            if(_configuration.SimulateDevice)
                return _configuration.SimulateDeviceType == DeviceType.Mobile;
#endif
            return GP_Device.IsMobile();
        }

        protected override bool IsDesktopDevice()
        {
#if UNITY_EDITOR
            if(_configuration.SimulateDevice)
                return _configuration.SimulateDeviceType == DeviceType.Desktop;
#endif
            return GP_Device.IsDesktop();
        }
    }
}