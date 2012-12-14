using System.Collections.Generic;

namespace NServiceBus.Profiler.Core
{
    public interface INetworkOperations
    {
        IList<string> GetMachines();
        void Browse(string productUrl);
    }
}