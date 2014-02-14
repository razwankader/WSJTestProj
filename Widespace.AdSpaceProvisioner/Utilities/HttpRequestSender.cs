using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Widespace.AdSpaceProvisioner
{
    public class HttpRequestSender : HttpRequestSenderListener, IHttpRequestSender
    {
        public HttpRequestSender() { }

        public string GetServiceURL()
        {
            UriBuilder baseUri = new UriBuilder(ServiceConstants.BASE_URL);
            string queryToAppend = string.Format("{0}={1}&{2}={3}&{4}={5}", ServiceConstants.APP_ID_KEY, ServiceConstants.APP_ID_VALUE, ServiceConstants.SDK_VERSION_KEY, ServiceConstants.SDK_VERSION_VALUE, ServiceConstants.PLATFORM_KEY, ServiceConstants.PLATFORM_VALUE);
            baseUri.Query = queryToAppend;
            return baseUri.Uri.ToString();
        }

        public void SendAsyncRequest(string serviceUrl, HttpRequestComleteHandler httpRequestComleted)
        {
            if (!Uri.IsWellFormedUriString(serviceUrl, UriKind.RelativeOrAbsolute))
                throw new ArgumentException("Invalid URL");

            base.AddListener(httpRequestComleted);

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
                httpWebRequest.Method = "GET";
                httpWebRequest.AllowReadStreamBuffering = false;
                httpWebRequest.Accept = "text/json";
                httpWebRequest.Timeout = 3000;
                httpWebRequest.BeginGetResponse(ResponseCallback, httpWebRequest);
            }
            catch
            {
                throw new TimeoutException("Request Timeout");
            }
        }

        private void ResponseCallback(IAsyncResult asyncResult)
        {
            string response = string.Empty;

            HttpWebRequest httpWebRequest = (HttpWebRequest)asyncResult.AsyncState;
            HttpWebResponse httpWebResponse = null;
            try
            {
                httpWebResponse = (HttpWebResponse)httpWebRequest.EndGetResponse(asyncResult);

                using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                httpWebResponse.Close();

                this.Notify(response);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Failed to parse web service replied data");
            }
            catch (Exception ex)
            {
                throw new TimeoutException("Request Timeout", ex.InnerException);
            }
        }

        public Dictionary<string, string> ParseResponse(string json)
        {
            try
            {
                JObject parsedJson = JObject.Parse(json);
                Dictionary<string, string> provisioningData = new Dictionary<string, string>();
                var sdkEnabled = parsedJson[ServiceConstants.IS_SDK_ENABLED];
                if (sdkEnabled != null)
                {
                    provisioningData.Add(ServiceConstants.IS_SDK_ENABLED, sdkEnabled.ToString());
                }
                var key = parsedJson[ServiceConstants.SESSION_INFO][ServiceConstants.SESSION_CRYPTO_KEY];
                if (key != null)
                {
                    provisioningData.Add(ServiceConstants.SESSION_CRYPTO_KEY, key.ToString());
                }
                var keyIndex = parsedJson[ServiceConstants.SESSION_INFO][ServiceConstants.SESSION_KEY_INDEX];
                if (keyIndex != null)
                {
                    provisioningData.Add(ServiceConstants.SESSION_KEY_INDEX, keyIndex.ToString());
                }

                return provisioningData;
            }
            catch
            {
                throw new ArgumentException("Failed to parse web service replied data");
            }
        }
    }
}
