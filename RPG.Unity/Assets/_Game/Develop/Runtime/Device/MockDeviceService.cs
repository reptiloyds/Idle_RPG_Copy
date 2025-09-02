using UnityEngine.Device;

namespace PleasantlyGames.RPG.Runtime.Device
{
    public class MockDeviceService : IDeviceService
    {
        private readonly DeviceType _constantDeviceType;

        public MockDeviceService(DeviceType deviceType) => 
            _constantDeviceType = deviceType;

        public DeviceType GetDevice()
        {
            if(_constantDeviceType != DeviceType.None)
                return _constantDeviceType;
// #if UNITY_WEBGL
//             return GetDeviceByScreenSize();
#if UNITY_IPHONE || UNITY_ANDROID
            return DeviceType.Mobile;
#endif
            return DeviceType.Desktop;
        }


        public OrientationType GetOrientation() => 
            Screen.width > Screen.height ? OrientationType.Horizontal : OrientationType.Vertical;
    }
}