using System.Collections.Generic;

namespace Widespace.AdSpaceProvisioner
{
    public interface IHttpRequestSender
    {
        string GetServiceURL();
        void SendAsyncRequest(string serviceUrl, Widespace.AdSpaceProvisioner.HttpRequestSenderListener.HttpRequestComleteHandler httpRequestComleted);
        Dictionary<string, string> ParseResponse(string json);
        void AddListener(Widespace.AdSpaceProvisioner.HttpRequestSenderListener.HttpRequestComleteHandler httpRequestComletedCallback);
        void RemoveListener(Widespace.AdSpaceProvisioner.HttpRequestSenderListener.HttpRequestComleteHandler httpRequestComletedCallback);
    }
}
