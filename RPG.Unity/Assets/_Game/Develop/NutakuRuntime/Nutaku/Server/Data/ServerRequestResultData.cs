namespace _Game.Scripts.Systems.Server.Data
{
    public class ServerRequestResultData
    {
        public ServerRequestResult ResultType;
        public string Result;
        public string Text;
    }

    public enum ServerRequestResult
    {
        None = 0,
        Success = 1,
        Error = 2,
    }
}