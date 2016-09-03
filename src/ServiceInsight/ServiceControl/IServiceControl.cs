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

        void RetryMessage(string messageId);

        Uri CreateServiceInsightUri(StoredMessage message);

        SagaData GetSagaById(Guid sagaId);

        PagedResult<StoredMessage> Search(string searchQuery, int pageIndex = 1, string orderBy = null, bool ascending = false);

        PagedResult<StoredMessage> GetAuditMessages(Endpoint endpoint, string searchQuery = null, int pageIndex = 1, string orderBy = null, bool ascending = false);

        IEnumerable<StoredMessage> GetConversationById(string conversationId);

        IEnumerable<KeyValuePair<string, string>> GetMessageData(SagaMessage messageId);

        void LoadBody(StoredMessage message);
    }

    public class ServiceControlHeaders
    {
        public const string ParticularVersion = "X-Particular-Version";
        public const string TotalCount = "Total-Count";
    }
}