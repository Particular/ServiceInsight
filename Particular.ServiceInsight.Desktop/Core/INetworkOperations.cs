using System.Collections.Generic;
using System.Threading.Tasks;

namespace Particular.ServiceInsight.Desktop.Core
{
    public interface INetworkOperations
    {
        Task<IList<string>> GetMachines();
        void Browse(string productUrl);
    }
}