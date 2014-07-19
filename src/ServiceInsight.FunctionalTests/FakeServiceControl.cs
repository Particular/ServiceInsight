namespace Particular.ServiceInsight.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using Desktop.Models;
    using Desktop.Saga;
    using Desktop.ServiceControl;

    public class FakeServiceControl : IServiceControl
    {
        public const string Version = "1.0-fake";

        public bool IsAlive()
        {
            return true;
        }

        public string GetVersion()
        {
            return Version;
        }

        public void RetryMessage(string messageId)
        {
        }

        public Uri CreateServiceInsightUri(StoredMessage message)
        {
            return new Uri(string.Format("si://localhost/api{0}", message.GetURIQuery()));
        }

        public bool HasSagaChanged(Guid sagaId)
        {
            return true;
        }

        public SagaData GetSagaById(Guid sagaId)
        {
            return new SagaData();
        }

        public PagedResult<StoredMessage> Search(string searchQuery, int pageIndex = 1, string orderBy = null, bool @ascending = false)
        {
            return new PagedResult<StoredMessage>();
        }

        public PagedResult<StoredMessage> GetAuditMessages(Endpoint endpoint, string searchQuery = null, int pageIndex = 1, string orderBy = null, bool @ascending = false)
        {
            return new PagedResult<StoredMessage>();
        }

        public IEnumerable<StoredMessage> GetConversationById(string conversationId)
        {
            return new List<StoredMessage>();
        }

        public IEnumerable<Endpoint> GetEndpoints()
        {
            return new List<Endpoint>();
        }

        public IEnumerable<KeyValuePair<string, string>> GetMessageData(Guid messageId)
        {
            return new List<KeyValuePair<string, string>>();
        }

        public void LoadBody(StoredMessage message)
        {
        }
    }
}