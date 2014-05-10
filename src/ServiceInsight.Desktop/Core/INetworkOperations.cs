namespace Particular.ServiceInsight.Desktop.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface INetworkOperations
    {
        Task<IList<string>> GetMachines();
        void Browse(string productUrl);
    }
}