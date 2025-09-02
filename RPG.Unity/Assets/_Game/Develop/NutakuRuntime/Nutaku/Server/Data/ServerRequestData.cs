using System;

namespace _Game.Scripts.Systems.Server.Data
{
    public class ServerRequestData
    {
        public readonly string Data;
        public readonly string AdditionalUrlPath;
        public readonly Action<ServerRequestResultData> CallBack;
        
        public RequestHeaderData HeaderData { get; private set; }
        public string Body { get; private set; }

        public ServerRequestData(string data, Action<ServerRequestResultData> callBack = null, string additionalUrlPath = "", RequestHeaderData headerData = null)
        {
            Data = data;
            CallBack = callBack;
            AdditionalUrlPath = additionalUrlPath;
            HeaderData = headerData;
        }

        public ServerRequestData SetHeader(RequestHeaderData data)
        {
            HeaderData = data;
            return this;
        }

        public ServerRequestData SetBody(string value)
        {
            Body = value;
            return this;
        }
    }

    public class RequestHeaderData
    {
        public string HeaderName;
        public string HeaderData;
    }
}