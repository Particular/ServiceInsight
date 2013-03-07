using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Profiler.Common.Models;
using RestSharp;
using NServiceBus.Profiler.Common.ExtensionMethods;

namespace NServiceBus.Profiler.Core.Management
{
    public class DefaultManagementService : IManagementService
    {
        public async Task<List<StoredMessage>> GetErrorMessages(string serviceUrl)
        {
            var client = new RestClient(serviceUrl);
            var request = new RestRequest("failedmessages");
            var messages = await client.GetModelAsync<List<StoredMessage>>(request);

            return messages;
        }

        public async Task<List<StoredMessage>> GetAuditMessages(string serviceUrl, Endpoint endpoint)
        {
            var client = new RestClient(serviceUrl);
            var request = new RestRequest("/endpoints/" + endpoint.Name + "/audit");
            var messages = await client.GetModelAsync<List<StoredMessage>>(request);

            return messages;            
        }

        public async Task<List<StoredMessage>> GetConversationById(string serviceUrl, string conversationId)
        {
            var client = new RestClient(serviceUrl);
            var request = new RestRequest("conversations/" + conversationId);
            var messages = await client.GetModelAsync<List<StoredMessage>>(request);

            return messages;
        }

        public async Task<List<Endpoint>> GetEndpoints(string serviceUrl)
        {
            var client = new RestClient(serviceUrl);
            var request = new RestRequest("endpoints");
            var messages = await client.GetModelAsync<List<Endpoint>>(request);

            return messages;
        }

        public async Task<bool> IsAlive(string serviceUrl)
        {
            var client = new RestClient(serviceUrl);
            var request = new RestRequest("ping");
            var version = await client.GetModelAsync<VersionInfo>(request);

            return version != null;
        }
    }
}