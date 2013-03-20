using System.Collections.Generic;
using System.Threading.Tasks;

namespace NServiceBus.Profiler.Core
{
    public interface INetworkOperations
    {
        Task<IList<string>> GetMachines();
        void Browse(string productUrl);
    }
}