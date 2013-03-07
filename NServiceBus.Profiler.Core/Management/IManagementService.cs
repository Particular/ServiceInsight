using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Core.Management
{
    public interface IManagementService
    {
        Task<List<StoredMessage>> GetErrorMessages(string serviceUrl);
        Task<List<StoredMessage>> GetAuditMessages(string serviceUrl, Endpoint endpoint);
        Task<List<StoredMessage>> GetConversationById(string serviceUrl, string conversationId);
        Task<List<Endpoint>> GetEndpoints(string serviceUrl);
        Task<bool> IsAlive(string serviceUrl);
    }
}