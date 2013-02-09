using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Core.Management
{
    public interface IManagementService
    {
        Task<List<TransportMessage>> GetErrorMessages(Endpoint endpoint);
        Task<List<TransportMessage>> GetAuditMessages(Endpoint endpoint);
        Task<bool> IsAlive(string connectedToService);
    }
}