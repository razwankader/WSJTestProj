using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Widespace.AdSpaceProvisioner
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Provisioner : ProvisionListener, IProvisioner
    {
        private static volatile Provisioner provisionerInstance;
        private static object syncRoot = new Object();
        private static Dictionary<string, string> provisioningData = new Dictionary<string, string>();
        private IHttpRequestSender httpRequestSender;

        private Provisioner() { }

        private Provisioner(IHttpRequestSender httpRequestSender)
        {
            this.httpRequestSender = httpRequestSender;
            ResetStatus();
        }

        public static Provisioner ProvisionerInstance
        {
            get
            {
                if (provisionerInstance == null)
                {
                    lock (syncRoot)
                    {
                        if (provisionerInstance == null)
                        {
                            provisionerInstance = new Provisioner(new HttpRequestSender());
                        }
                    }
                }

                return provisionerInstance;
            }
        }

        public void InjectDependentInterface(IHttpRequestSender provisionService)
        {
            this.httpRequestSender = provisionService;
        }

        private ProvisionStatus status;
        public ProvisionStatus Status
        {
            get { return status; }
            private set
            {
                status = value;
                if (status == ProvisionStatus.DONE || status == ProvisionStatus.FAILED)
                {
                    Notify();
                }
            }
        }

        public void Provision()
        {
            Status = ProvisionStatus.IN_PROGRESS;
            try
            {
                httpRequestSender.SendAsyncRequest(httpRequestSender.GetServiceURL(), OnHttpRequestCompleted);
            }
            catch (TimeoutException)
            {
                Status = ProvisionStatus.FAILED;
            }
            catch (ArgumentException)
            {
                Status = ProvisionStatus.FAILED;
            } 
        }

        public string ReadKey(string key)
        {
            if (!provisioningData.ContainsKey(key))
            {
                throw new ArgumentNullException("Key: " + key + " is not exists in the dictionary");
            }

            return provisioningData[key];
        }

        public void StoreKey(string key, string value)
        {
            if (provisioningData.ContainsKey(key))
            {
                throw new ArgumentException("Key: " + key + " is already exists in the dictionary");
            }

            provisioningData.Add(key, value);
        }

        public void ResetStatus()
        {
            Status = ProvisionStatus.UNPROVISIONED;
        }

        private void OnHttpRequestCompleted(string response)
        {
            provisioningData = httpRequestSender.ParseResponse(response);
            httpRequestSender.RemoveListener(OnHttpRequestCompleted);
            Status = ProvisionStatus.DONE;
        }
    }
}
