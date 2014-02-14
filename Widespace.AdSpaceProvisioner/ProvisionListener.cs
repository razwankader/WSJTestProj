using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widespace.AdSpaceProvisioner
{
    public abstract class ProvisionListener
    {
        private event Action provisionListener;

        public void AddListener(Action provisionListener)
        {
            this.provisionListener += provisionListener;
        }

        public void RemoveListener(Action provisionListener)
        {
            this.provisionListener -= provisionListener;
        }

        public void Notify()
        {
            if (this.provisionListener != null)
            {
                this.provisionListener();
            }
        }

        public int GetListenerCount()
        {
            if (this.provisionListener == null)
                return 0;
            else
                return this.provisionListener.GetInvocationList().Length;
        }
    }


}
