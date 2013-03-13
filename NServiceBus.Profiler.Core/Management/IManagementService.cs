using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Core.Management
{
    public interface IManagementService
    {
        Task<PagedResult<StoredMessage>> GetErrorMessages(string serviceUrl);
        Task<PagedResult<StoredMessage>> GetAuditMessages(string serviceUrl, Endpoint endpoint, string searchQuery = null, int pageIndex = 1);
        Task<List<StoredMessage>> GetConversationById(string serviceUrl, string conversationId);
        Task<List<Endpoint>> GetEndpoints(string serviceUrl);
        Task<bool> IsAlive(string serviceUrl);
    }
}