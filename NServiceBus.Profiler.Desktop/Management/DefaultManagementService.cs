using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Caliburn.PresentationFramework.ApplicationModel;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core.MessageDecoders;
using NServiceBus.Profiler.Desktop.Events;
using RestSharp;
using log4net;

namespace NServiceBus.Profiler.Desktop.Management
{
    public class DefaultManagementService : IManagementService
    {
        private readonly IManagementConnectionProvider _connection;
        private readonly IEventAggregator _eventAggregator;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(IRestClient));

        public DefaultManagementService(
            IManagementConnectionProvider connection, 
            IEventAggregator eventAggregator)
        {
            _connection = connection;
            _eventAggregator = eventAggregator;
        }

        public async Task<PagedResult<StoredMessage>> GetErrorMessages()
        {
            var request = new RestRequest("failedmessages");
            var result = await GetPagedResult<StoredMessage>(request);
            
            return result;
        }

        public async Task<PagedResult<StoredMessage>> Search(string searchKeyword, int pageIndex = 1)
        {
            var request = new RestRequest("/messages/");
            
            AppendSearchQuery(request, searchKeyword);
            AppendPaging(request, pageIndex);

            var result = await GetPagedResult<StoredMessage>(request);
            result.CurrentPage = pageIndex;

            return result;
        }

        public async Task<PagedResult<StoredMessage>> GetAuditMessages(Endpoint endpoint, string searchQuery = null, int pageIndex = 1, string orderBy = null, bool ascending = false)
        {
            var request = new RestRequest(CreateBaseUrl(endpoint.Name, searchQuery));

            AppendSearchQuery(request, searchQuery);
            AppendPaging(request, pageIndex);
            AppendOrdering(request, orderBy, ascending);

            var result = await GetPagedResult<StoredMessage>(request);
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
            var messages = await GetModelAsync<List<StoredMessage>>(request) ?? new List<StoredMessage>();

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
            var messages = await GetModelAsync<List<Endpoint>>(request);

            return messages ?? new List<Endpoint>();
        }

        public async Task<bool> IsAlive()
        {
            return await GetVersion() != null;
        }

        public async Task<VersionInfo> GetVersion()
        {
            var request = new RestRequest("ping");
            var version = await GetModelAsync<VersionInfo>(request);

            return version;
        }

        public async Task<bool> RetryMessage(string messageId)
        {
            var request = new RestRequest("errors/" + messageId + "/retry", Method.POST);
            var response = await ExecuteAsync(request);

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

        private Task<PagedResult<T>> GetPagedResult<T>(IRestRequest request) where T : class, new()
        {
            var completionSource = new TaskCompletionSource<PagedResult<T>>();
            var client = CreateClient();

            client.ExecuteAsync<List<T>>(request, response =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    completionSource.SetResult(new PagedResult<T>
                    {
                        Result = response.Data,
                        TotalCount = int.Parse(response.Headers.First(x => x.Name == "Total-Count").Value.ToString())
                    });
                }
                else
                {
                    RaiseAsyncOperationFailed(response.StatusCode, response.ErrorMessage);
                    var errorMessage = string.Format("Unknown error connecting to the service at {0}, Http Status code is {1}", client.BuildUri(request), response.StatusCode);
                    Logger.Error(errorMessage, response.ErrorException);
                    completionSource.SetException(response.ErrorException);
                }
            });
            return completionSource.Task;
        }

        private void RaiseAsyncOperationFailed(HttpStatusCode statusCode, string errorMessage)
        {
            _eventAggregator.Publish(new AsyncOperationFailedEvent
            {
                ErrorCode = (int)statusCode,
                Description = errorMessage
            });
        }

        private Task<T> GetModelAsync<T>(IRestRequest request)
            where T : class, new()
        {
            return ExecuteAsync<T>(request, response => response.Data);
        }

        private Task<bool> ExecuteAsync(IRestRequest request)
        {
            var completionSource = new TaskCompletionSource<bool>();
            CreateClient().ExecuteAsync(request, response => ProcessResponse(r => response.StatusCode == HttpStatusCode.OK, response, completionSource));
            return completionSource.Task;
        }

        private Task<T> ExecuteAsync<T>(IRestRequest request, Func<IRestResponse<T>, T> selector)
            where T : class, new()
        {
            var completionSource = new TaskCompletionSource<T>();
            CreateClient().ExecuteAsync<T>(request, response => ProcessResponse(selector, response, completionSource));
            return completionSource.Task;
        }

        private void ProcessResponse<T>(Func<IRestResponse, T> selector, IRestResponse response, TaskCompletionSource<T> completionSource)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                completionSource.SetResult(selector(response));
            }
            else
            {
                RaiseAsyncOperationFailed(response.StatusCode, response.ErrorMessage);
                var errorMessage = string.Format("Error executing the request, Http Status code is {0}", response.StatusCode);
                Logger.Error(errorMessage, response.ErrorException);
                completionSource.SetResult(default(T));
            }
        }

        private void ProcessResponse<T>(Func<IRestResponse<T>, T> selector, IRestResponse<T> response, TaskCompletionSource<T> completionSource)
            where T : class, new()
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                completionSource.SetResult(selector(response));
            }
            else
            {
                RaiseAsyncOperationFailed(response.StatusCode, response.ErrorMessage);
                var errorMessage = string.Format("Error executing the request, Http Status code is {0}", response.StatusCode);
                Logger.Error(errorMessage, response.ErrorException);
                completionSource.SetResult(null);
            }
        }

    }
}