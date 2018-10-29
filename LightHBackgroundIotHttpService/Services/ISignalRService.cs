namespace LightHBackgroundIotHttpService.Services
{
    public interface ISignalRService
    {
        void Connect();
        void InvokeSendStatusMethod(string isOn);
        void InvokeSendStaticticData(int timeOn, int timeOf);
        void InvokeTurnOnLight(bool isOn);
    }
}
