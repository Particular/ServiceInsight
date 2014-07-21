namespace Particular.ServiceInsight.FunctionalTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Desktop.Models;
    using Desktop.Saga;
    using Desktop.ServiceControl;
    using Newtonsoft.Json;

    public class FakeServiceControl : IServiceControl
    {
        public const string Version = "1.0-fake";
        public const string Address = "localhost:1234";

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
            return new Uri(string.Format("si://{0}/api{1}", Address, message.GetURIQuery()));
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
            return Get<IList<Endpoint>>("GetEndpoints");
        }

        public IEnumerable<KeyValuePair<string, string>> GetMessageData(Guid messageId)
        {
            return new List<KeyValuePair<string, string>>();
        }

        public void LoadBody(StoredMessage message)
        {
        }

        private T Get<T>(string scenarioName)
        {
            var scenarioFile = Path.Combine(TestDataFolder, scenarioName + ".json");
            if (File.Exists(scenarioFile))
            {
                var scenarioContent = File.ReadAllText(scenarioFile);
                return JsonConvert.DeserializeObject<T>(scenarioContent);
            }

            return default(T);
        }

        private string TestDataFolder
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData"); }
        }
    }
}