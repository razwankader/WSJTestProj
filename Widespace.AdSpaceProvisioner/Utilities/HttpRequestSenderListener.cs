using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widespace.AdSpaceProvisioner
{
    public abstract class HttpRequestSenderListener
    {
        public delegate void HttpRequestComleteHandler(string result);
        private event HttpRequestComleteHandler HttpRequestComleted;

        public void AddListener(HttpRequestComleteHandler httpRequestComletedCallback)
        {
            this.HttpRequestComleted += httpRequestComletedCallback;
        }

        public void RemoveListener(HttpRequestComleteHandler httpRequestComletedCallback)
        {
            this.HttpRequestComleted -= httpRequestComletedCallback;
        }

        public void Notify(string response)
        {
            if (this.HttpRequestComleted != null)
            {
                this.HttpRequestComleted(response);
            }
        }

        public int GetListenerCount()
        {
            if (this.HttpRequestComleted == null)
                return 0;
            else
                return this.HttpRequestComleted.GetInvocationList().Length;
        }
    }
}
