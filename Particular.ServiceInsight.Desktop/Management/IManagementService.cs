﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.Management
{
    public interface IManagementService
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