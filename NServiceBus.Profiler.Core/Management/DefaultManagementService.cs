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
            var request = new RestRequest("/messages/");
            
            AppendSearchQuery(request, searchKeyword);
            AppendPaging(request, pageIndex);

            var result = await client.GetPagedResult<StoredMessage>(request);
            result.CurrentPage = pageIndex;

            return result;
        }

        public async Task<PagedResult<StoredMessage>> GetAuditMessages(string serviceUrl, Endpoint endpoint, string searchQuery = null, int pageIndex = 1, string orderBy = null, bool ascending = false)
        {
            var client = new RestClient(serviceUrl);
            var request = new RestRequest(CreateBaseUrl(endpoint.Name, searchQuery));

            AppendSearchQuery(request, searchQuery);
            AppendPaging(request, pageIndex);
            AppendOrdering(request, orderBy, ascending);

            var result = await client.GetPagedResult<StoredMessage>(request);
            result.CurrentPage = pageIndex;

            return result;
        }

        private static string CreateBaseUrl(string endpointName, string searchQuery)
        {
            return searchQuery == null ? string.Format("/endpoints/{0}/messages/", endpointName) 
                                       : "/messages/";
        }

        public async Task<List<StoredMessage>> GetConversationById(string serviceUrl, string conversationId)
        {
            var client = new RestClient(serviceUrl);
            var request = new RestRequest(string.Format("conversations/{0}", conversationId));
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

        public async Task<bool> RetryMessage(string serviceUrl, string messageId)
        {
            var client = new RestClient(serviceUrl);
            var request = new RestRequest("errors/retry", Method.POST);
            request.AddParameter("MessageId", messageId);
            var response = await client.ExecuteAsync(request);

            return response;
        }

        private void AppendOrdering(IRestRequest request, string orderBy, bool ascending)
        {
            if(orderBy == null) return;
            request.AddParameter("sort", orderBy, ParameterType.GetOrPost);
            request.AddParameter("direction", GetSortDirection(ascending), ParameterType.GetOrPost);
        }

        private void AppendPaging(IRestRequest request, int pageIndex)
        {
            request.AddParameter("page", pageIndex, ParameterType.GetOrPost);
        }

        private void AppendSearchQuery(IRestRequest request, string searchQuery)
        {
            if(searchQuery == null) return;
            request.Resource += string.Format("search/{0}", searchQuery);
        }

        private string GetSortDirection(bool ascending)
        {
            return ascending ? "asc" : "desc";
        }
    }
}