using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Core.Management
{
    public interface IManagementService
    {
        Task<List<StoredMessage>> GetErrorMessages(Endpoint endpoint);
        Task<List<StoredMessage>> GetAuditMessages(Endpoint endpoint);
        Task<List<StoredMessage>> GetConversationById(Endpoint endpoint, string conversationId);
        Task<bool> IsAlive(string connectedToService);
    }
}