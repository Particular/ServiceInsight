using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Core.Management
{
    public interface IManagementService
    {
        Task<PagedResult<StoredMessage>> GetErrorMessages(string serviceUrl);
        Task<PagedResult<StoredMessage>> GetAuditMessages(string serviceUrl, Endpoint endpoint, string searchQuery = null, int pageIndex = 1, string orderBy = null, bool ascending = false);
        Task<List<StoredMessage>> GetConversationById(string serviceUrl, string conversationId);
        Task<List<Endpoint>> GetEndpoints(string serviceUrl);
        Task<bool> RetryMessage(string serviceUrl, string messageId);
        Task<bool> IsAlive(string serviceUrl);
    }
}