namespace Particular.ServiceInsight.FunctionalTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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

        public SagaData GetSagaById(Guid sagaId)
        {
            return new SagaData();
        }

        public PagedResult<StoredMessage> Search(string searchQuery, int pageIndex = 1, string orderBy = null, bool @ascending = false)
        {
            const int PageSize = 10;

            var messages = Get<List<StoredMessage>>("Messages");
            var pagedResult = new PagedResult<StoredMessage>
            {
                Result = messages.Skip(pageIndex - 1*10).Take(PageSize).ToList(),
                TotalCount = messages.Count
            };

            return pagedResult;
        }

        public PagedResult<StoredMessage> GetAuditMessages(Endpoint endpoint, string searchQuery = null, int pageIndex = 1, string orderBy = null, bool @ascending = false)
        {
            const int PageSize = 10;

            var messages = Get<List<StoredMessage>>(string.Format("{0}-Messages", endpoint.Name));
            var pagedResult = new PagedResult<StoredMessage>
            {
                Result = messages.Skip(pageIndex - 1 * 10).Take(PageSize).ToList(),
                TotalCount = messages.Count
            };

            return pagedResult;            
        }

        public IEnumerable<StoredMessage> GetConversationById(string conversationId)
        {
            var messages = Get<List<StoredMessage>>("Conversation-Messages");
            return new List<StoredMessage>(messages);
        }

        public IEnumerable<Endpoint> GetEndpoints()
        {
            return Get<List<Endpoint>>("GetEndpoints");
        }

        public Tuple<string, string> GetMessageData(SagaMessage message)
        {
            return null;
        }

        public void LoadBody(StoredMessage message)
        {
        }

        private T Get<T>(string scenarioName) where T : new()
        {
            var scenarioFile = Path.Combine(TestDataFolder, scenarioName + ".json");
            if (File.Exists(scenarioFile))
            {
                var scenarioContent = File.ReadAllText(scenarioFile);
                return JsonConvert.DeserializeObject<T>(scenarioContent);
            }

            return new T();
        }

        private string TestDataFolder
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData"); }
        }
    }
}
