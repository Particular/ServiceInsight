namespace Particular.ServiceInsight.Desktop.ServiceControl
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Saga;

    public interface IServiceControl
    {
        PagedResult<StoredMessage> Search(string searchQuery, int pageIndex = 1, string orderBy = null, bool ascending = false);

        PagedResult<StoredMessage> GetAuditMessages(Endpoint endpoint, string searchQuery = null, int pageIndex = 1, string orderBy = null, bool ascending = false);

        List<StoredMessage> GetConversationById(string conversationId);

        List<Endpoint> GetEndpoints();

        bool IsAlive();

        string GetVersion();

        bool RetryMessage(string messageId);

        string GetBody(string bodyUrl);

        Uri GetUri(StoredMessage message);

        bool HasSagaChanged(string sagaId);

        SagaData GetSagaById(string sagaId);
    }

    public class ServiceControlHeaders
    {
        public const string ParticularVersion = "X-Particular-Version";
        public const string TotalCount = "Total-Count";
    }
}
