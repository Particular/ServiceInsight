using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Management
{
    public interface IServiceControl
    {
        Task<PagedResult<StoredMessage>> GetErrorMessages();
        Task<PagedResult<StoredMessage>> GetAuditMessages(Endpoint endpoint, string searchQuery = null, int pageIndex = 1, string orderBy = null, bool ascending = false);
        Task<List<StoredMessage>> GetConversationById(string conversationId);
        Task<List<Endpoint>> GetEndpoints();
        Task<bool> RetryMessage(string messageId);
        Task<bool> IsAlive();
        Task<string> GetVersion();
    }
}