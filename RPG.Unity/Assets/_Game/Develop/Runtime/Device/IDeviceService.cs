namespace PleasantlyGames.RPG.Runtime.Device
{
    public interface IDeviceService
    {
        DeviceType GetDevice();
        OrientationType GetOrientation();
    }
}