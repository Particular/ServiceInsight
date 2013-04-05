using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Profiler.Common.Models;
using RestSharp;
using NServiceBus.Profiler.Common.ExtensionMethods;

namespace NServiceBus.Profiler.Core.Management
{
    public class DefaultManagementService : IManagementService
    {
        public async Task<PagedResult<StoredMessage>> GetErrorMessages(string serviceUrl)
        {
            var client = new RestClient(serviceUrl);
            var request = new RestRequest("failedmessages");
            var result = await client.GetPagedResult<StoredMessage>(request);
            
            return result;
        }

        public async Task<PagedResult<StoredMessage>> Search(string serviceUrl, string searchKeyword, int pageIndex = 1)
        {
            var client = new RestClient(serviceUrl);
            var request = new RestRequest("/messages/search/" + searchKeyword + "?page=" + pageIndex);
            var result = await client.GetPagedResult<StoredMessage>(request);
            result.CurrentPage = pageIndex;

            return result;
        }

        public async Task<PagedResult<StoredMessage>> GetAuditMessages(string serviceUrl, Endpoint endpoint, string searchQuery = null, int pageIndex = 1)
        {
            var client = new RestClient(serviceUrl);
            var query = searchQuery != null ? CreateSearchQuery(endpoint.Name, searchQuery, pageIndex) : 
                                              CreateFetchQuery(endpoint.Name, pageIndex);
            var request = new RestRequest(query);
            var result = await client.GetPagedResult<StoredMessage>(request);
            result.CurrentPage = pageIndex;

            return result;
        }

        public async Task<List<StoredMessage>> GetConversationById(string serviceUrl, string conversationId)
        {
            var client = new RestClient(serviceUrl);
            var request = new RestRequest("conversations/" + conversationId);
            var messages = await client.GetModelAsync<List<StoredMessage>>(request);

            return messages ?? new List<StoredMessage>();
        }

        public async Task<List<Endpoint>> GetEndpoints(string serviceUrl)
        {
            var client = new RestClient(serviceUrl);
            var request = new RestRequest("endpoints");
            var messages = await client.GetModelAsync<List<Endpoint>>(request);

            return messages ?? new List<Endpoint>();
        }

        public async Task<bool> IsAlive(string serviceUrl)
        {
            var client = new RestClient(serviceUrl);
            var request = new RestRequest("ping");
            var version = await client.GetModelAsync<VersionInfo>(request);

            return version != null;
        }

        private string CreateSearchQuery(string endpoint, string searchQuery, int pageIndex)
        {
            return string.Format("/endpoint/{0}/messages/search/{1}?page={2}", endpoint, searchQuery, pageIndex);
        }

        private string CreateFetchQuery(string endpointName, int pageIndex)
        {
            return string.Format("/endpoints/{0}/audit?page={1}", endpointName, pageIndex);
        }
    }
}