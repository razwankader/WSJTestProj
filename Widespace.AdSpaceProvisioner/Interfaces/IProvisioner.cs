using System;

namespace Widespace.AdSpaceProvisioner
{
    public interface IProvisioner
    {
        ProvisionStatus Status { get; }
        void Provision();
        void AddListener(Action provisionListener);
        void RemoveListener(Action provisionListener);
        string ReadKey(string key);
        void StoreKey(string key, string value);
        void ResetStatus();
    }
}
