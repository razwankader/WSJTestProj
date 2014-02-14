using System;
using System.Collections.Generic;

namespace Widespace.AdSpaceProvisioner
{
    public class AdSpace
    {
        private Queue<Action> prefetchAdActions = new Queue<Action>();
        private Action latestRunAdAction;

        private IProvisioner provisioner;

        public AdSpace()
        {
            provisioner = Provisioner.ProvisionerInstance;
            provisioner.AddListener(UpdateAdSpace);
        }

        private void Init()
        {
            provisioner.Provision();
        }

        public void RunAd(Action resultCallback)
        {
            if (provisioner.Status == ProvisionStatus.DONE)
            {
                resultCallback();
                return;
            }
            else if (provisioner.Status == ProvisionStatus.UNPROVISIONED
                    || provisioner.Status == ProvisionStatus.FAILED)
            {
                Init();
            }

            latestRunAdAction = () => RunAd(resultCallback);
        }

        public void PrefetchAd(Action resultCallback)
        {
            if (provisioner.Status == ProvisionStatus.DONE)
            {
                resultCallback();
                return;
            }
            else if (provisioner.Status == ProvisionStatus.UNPROVISIONED
                    || provisioner.Status == ProvisionStatus.FAILED)
            {
                Init();
            }

            prefetchAdActions.Enqueue(() => PrefetchAd(resultCallback));
        }

        private void UpdateAdSpace()
        {
            if (latestRunAdAction != null)
            {
                latestRunAdAction();
            }

            while (prefetchAdActions.Count != 0)
            {
                Action action = prefetchAdActions.Dequeue();
                action();
            }

            if(provisioner.Status == ProvisionStatus.DONE)
                provisioner.RemoveListener(UpdateAdSpace);
        }
    }
}
