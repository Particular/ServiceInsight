using NServiceBus.Profiler.Common.ExtensionMethods;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core.MessageDecoders;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NServiceBus.Profiler.Core.Management
{
    public class DefaultManagementService : IManagementService
    {
        private readonly IManagementConnectionProvider _connection;

        public DefaultManagementService(IManagementConnectionProvider connection)
        {
            _connection = connection;
        }

        public async Task<PagedResult<StoredMessage>> GetErrorMessages()
        {
            var request = new RestRequest("failedmessages");
            var result = await CreateClient().GetPagedResult<StoredMessage>(request);
            
            return result;
        }

        public async Task<PagedResult<StoredMessage>> Search(string searchKeyword, int pageIndex = 1)
        {
            var request = new RestRequest("/messages/");
            
            AppendSearchQuery(request, searchKeyword);
            AppendPaging(request, pageIndex);

            var result = await CreateClient().GetPagedResult<StoredMessage>(request);
            result.CurrentPage = pageIndex;

            return result;
        }

        public async Task<PagedResult<StoredMessage>> GetAuditMessages(Endpoint endpoint, string searchQuery = null, int pageIndex = 1, string orderBy = null, bool ascending = false)
        {
            var request = new RestRequest(CreateBaseUrl(endpoint.Name, searchQuery));

            AppendSearchQuery(request, searchQuery);
            AppendPaging(request, pageIndex);
            AppendOrdering(request, orderBy, ascending);

            var result = await CreateClient().GetPagedResult<StoredMessage>(request);
            result.CurrentPage = pageIndex;

            return result;
        }

        private static string CreateBaseUrl(string endpointName, string searchQuery)
        {
            return searchQuery == null ? string.Format("/endpoints/{0}/messages/", endpointName) 
                                       : "/messages/";
        }

        public async Task<List<StoredMessage>> GetConversationById(string conversationId)
        {
            var request = new RestRequest(string.Format("conversations/{0}", conversationId));
            var messages = await CreateClient().GetModelAsync<List<StoredMessage>>(request) ?? new List<StoredMessage>();

            //************* Workaround until API is fixed. Remove in Beta2 
            // http://particular.myjetbrains.com/youtrack/issue/SB-137
            var decoder = new HeaderContentDecoder(new StringContentDecoder());

            foreach (var storedMessage in messages)
            {
                try
                {
                    var result = decoder.Decode(storedMessage.Headers);

                    var messageIdHeader = result.Value.FirstOrDefault(h => h.Key == "NServiceBus.MessageId");

                    if (messageIdHeader != null)
                        storedMessage.MessageId = messageIdHeader.Value;
                    
                }
                catch(Exception){}
            }

            //************* End workaround

            return messages;
        }

        public async Task<List<Endpoint>> GetEndpoints()
        {
            var request = new RestRequest("endpoints");
            var messages = await CreateClient().GetModelAsync<List<Endpoint>>(request);

            return messages ?? new List<Endpoint>();
        }

        public async Task<bool> IsAlive()
        {
            return await GetVersion() != null;
        }

        public async Task<VersionInfo> GetVersion()
        {
            var request = new RestRequest("ping");
            var version = await CreateClient().GetModelAsync<VersionInfo>(request);

            return version;
        }

        public async Task<bool> RetryMessage(string messageId)
        {
            var request = new RestRequest("errors/" + messageId + "/retry", Method.POST);
            var response = await CreateClient().ExecuteAsync(request);

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

        private IRestClient CreateClient()
        {
            return new RestClient(_connection.Url);
        }
    }
}