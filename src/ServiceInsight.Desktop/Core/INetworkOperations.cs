using System.Collections.Generic;
using System.Threading.Tasks;

namespace NServiceBus.Profiler.Desktop.Core
{
    public interface INetworkOperations
    {
        Task<IList<string>> GetMachines();
        void Browse(string productUrl);
    }
}