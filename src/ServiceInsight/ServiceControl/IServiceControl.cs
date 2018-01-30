namespace ServiceInsight.ServiceControl
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Saga;

    public interface IServiceControl
    {
        bool IsAlive();

        string GetVersion();

        void RetryMessage(string messageId, string instanceId);

        Uri CreateServiceInsightUri(StoredMessage message);

        SagaData GetSagaById(Guid sagaId);

        PagedResult<StoredMessage> GetAuditMessages(Endpoint endpoint, string searchQuery = null, string orderBy = null, bool ascending = false);

        PagedResult<StoredMessage> GetAuditMessages(string link);

        IEnumerable<StoredMessage> GetConversationById(string conversationId);

        IEnumerable<Endpoint> GetEndpoints();

        IEnumerable<KeyValuePair<string, string>> GetMessageData(SagaMessage messageId);

        void LoadBody(StoredMessage message);
    }

    public class ServiceControlHeaders
    {
        public const string ParticularVersion = "X-Particular-Version";
        public const string TotalCount = "Total-Count";
        public const string Link = "Link";
        public const string PageSize = "Page-Size";
    }
}