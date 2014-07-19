namespace Particular.ServiceInsight.FunctionalTests.DiagramTests
{
    using System;
    using System.Collections.Generic;
    using Desktop.Models;
    using Desktop.Saga;
    using Desktop.ServiceControl;

    public class FakeServiceControl : IServiceControl
    {
        public bool IsAlive()
        {
            return true;
        }

        public string GetVersion()
        {
            return "Stub-v1.0";
        }

        public void RetryMessage(string messageId)
        {
        }

        public Uri CreateServiceInsightUri(StoredMessage message)
        {
            return new Uri("");
        }

        public bool HasSagaChanged(Guid sagaId)
        {
            return false;
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