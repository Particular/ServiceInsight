using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Profiler.Desktop.Models;
using System;
using NServiceBus.Profiler.Desktop.Saga;

namespace NServiceBus.Profiler.Desktop.ServiceControl
{
    public interface IServiceControl
    {
        Task<PagedResult<StoredMessage>> GetAuditMessages(Endpoint endpoint, string searchQuery = null, int pageIndex = 1, string orderBy = null, bool ascending = false);
        Task<PagedResult<StoredMessage>> Search(string searchQuery = null, int pageIndex = 1, string orderBy = null, bool ascending = false);
        Task<List<StoredMessage>> GetConversationById(string conversationId);
        Task<List<Endpoint>> GetEndpoints();
        Task<bool> RetryMessage(string messageId);
        Task<bool> IsAlive();
        Task<string> GetBody(string bodyUrl);
        Task<SagaData> GetSagaById(string sagaId);
        Task<string> GetVersion();
        Uri GetUri(StoredMessage message);
    }
}