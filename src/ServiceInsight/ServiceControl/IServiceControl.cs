namespace ServiceInsight.ServiceControl
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;
    using Saga;

    public interface IServiceControl
    {
        Task<(bool, string)> IsAlive();

        Task<(string, string)> GetVersion();

        Task RetryMessage(string messageId, string instanceId);

        Uri CreateServiceInsightUri(StoredMessage message);

        Task<SagaData> GetSagaById(Guid sagaId);

        Task<PagedResult<StoredMessage>> GetAuditMessages(Endpoint endpoint = null, int? pageNo = null, string searchQuery = null, string orderBy = null, bool ascending = false);

        Task<PagedResult<StoredMessage>> GetAuditMessages(string link);

        Task<IEnumerable<StoredMessage>> GetConversationById(string conversationId);

        Task<IEnumerable<Endpoint>> GetEndpoints();

        Task<IEnumerable<KeyValuePair<string, string>>> GetMessageData(SagaMessage messageId);

        Task LoadBody(StoredMessage message);
    }

    public class ServiceControlHeaders
    {
        public const string ParticularVersion = "X-Particular-Version";
        public const string TotalCount = "Total-Count";
        public const string Link = "Link";
        public const string PageSize = "Page-Size";
    }
}