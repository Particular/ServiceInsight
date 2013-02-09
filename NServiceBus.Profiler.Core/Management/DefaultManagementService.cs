using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Profiler.Common.Models;
using RestSharp;
using NServiceBus.Profiler.Common.ExtensionMethods;

namespace NServiceBus.Profiler.Core.Management
{
    public class DefaultManagementService : IManagementService
    {
        public async Task<List<TransportMessage>> GetErrorMessages(Endpoint endpoint)
        {
            var client = new RestClient(endpoint.Url);
            var request = new RestRequest("failedmessages");
            var messages = await client.GetModelAsync<List<TransportMessage>>(request);

            return messages;
        }

        public async Task<List<TransportMessage>> GetAuditMessages(Endpoint endpoint)
        {
            var client = new RestClient(endpoint.Url);
            var request = new RestRequest("auditmessages");
            var messages = await client.GetModelAsync<List<TransportMessage>>(request);

            return messages;            
        }

        public async Task<bool> IsAlive(string connectedToService)
        {
            var client = new RestClient(connectedToService);
            var request = new RestRequest("ping");
            var version = await client.GetModelAsync<VersionInfo>(request);

            return version != null;
        }
    }
}