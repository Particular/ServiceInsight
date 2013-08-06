using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Caliburn.PresentationFramework.ApplicationModel;
using NServiceBus.Profiler.Desktop.Core.Settings;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.Desktop.Settings;
using RestSharp;
using RestSharp.Contrib;
using log4net;

namespace NServiceBus.Profiler.Desktop.Management
{
    public class DefaultManagementService : IManagementService
    {
        private readonly IManagementConnectionProvider _connection;
        private readonly IEventAggregator _eventAggregator;
        private readonly ProfilerSettings _settings;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(IManagementService));

        public DefaultManagementService(
            IManagementConnectionProvider connection, 
            IEventAggregator eventAggregator,
            ISettingsProvider settingsProvider)
        {
            _connection = connection;
            _eventAggregator = eventAggregator;
            _settings = settingsProvider.GetSettings<ProfilerSettings>();
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

            AppendSystemMessages(request, searchQuery);
            AppendSearchQuery(request, searchQuery);
            AppendPaging(request, pageIndex);
            AppendOrdering(request, orderBy, ascending);

            var result = await GetPagedResult<StoredMessage>(request);
            result.CurrentPage = pageIndex;

            return result;
        }

        public async Task<List<StoredMessage>> GetConversationById(string conversationId)
        {
            var request = new RestRequest(string.Format("conversations/{0}", conversationId));
            var messages = await GetModelAsync<List<StoredMessage>>(request) ?? new List<StoredMessage>();

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

        public async Task<string> GetVersion()
        {
            var request = new RestRequest();

            LogRequest(request);

            var completionSource = new TaskCompletionSource<string>();
            CreateClient()
                .ExecuteAsync(request,
                              response =>
                              ProcessResponse(
                                  restResponse =>
                                  restResponse.Headers.First(x => x.Name == "X-Particular-Version").Value.ToString(),
                                  response, completionSource));

            return await completionSource.Task;
        }

        public async Task<bool> RetryMessage(string messageId)
        {
            var url = string.Format("errors/{0}/retry", messageId);
            var request = new RestRequest(url, Method.POST);
            var response = await ExecuteAsync(request);

            return response;
        }

        private void AppendSystemMessages(RestRequest request, string searchQuery)
        {
            if (searchQuery != null) return; //Not supported by search endpoint/api
            request.AddParameter("include_system_messages", _settings.DisplaySystemMessages);
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
            request.Resource += string.Format("search/{0}", Encode(searchQuery));
        }

        private string GetSortDirection(bool ascending)
        {
            return ascending ? "asc" : "desc";
        }

        private IRestClient CreateClient()
        {
            return new RestClient(_connection.Url);
        }

        private static string CreateBaseUrl(string endpointName, string searchQuery)
        {
            return searchQuery == null ? string.Format("endpoints/{0}/messages/", endpointName) : "messages/";
        }

        private Task<PagedResult<T>> GetPagedResult<T>(IRestRequest request) where T : class, new()
        {
            LogRequest(request);

            var completionSource = new TaskCompletionSource<PagedResult<T>>();
            var client = CreateClient();

            client.ExecuteAsync<List<T>>(request, response =>
            {
                LogResponse(response);

                if (HasSucceeded(response))
                {
                    completionSource.SetResult(new PagedResult<T>
                    {
                        Result = response.Data,
                        TotalCount = int.Parse(response.Headers.First(x => x.Name == "Total-Count").Value.ToString())
                    });
                }
                else
                {
                    LogError(response);
                    completionSource.SetResult(new PagedResult<T>());
                }
            });
            return completionSource.Task;
        }

        private Task<T> GetModelAsync<T>(IRestRequest request)
            where T : class, new()
        {
            return ExecuteAsync<T>(request, response => response.Data);
        }

        private Task<bool> ExecuteAsync(IRestRequest request)
        {
            LogRequest(request);

            var completionSource = new TaskCompletionSource<bool>();
            CreateClient().ExecuteAsync(request, response => ProcessResponse(HasSucceeded, response, completionSource));
            return completionSource.Task;
        }

        private Task<T> ExecuteAsync<T>(IRestRequest request, Func<IRestResponse<T>, T> selector)
            where T : class, new()
        {
            LogRequest(request);

            var completionSource = new TaskCompletionSource<T>();
            CreateClient().ExecuteAsync<T>(request, response => ProcessResponse(selector, response, completionSource));
            return completionSource.Task;
        }

        private void ProcessResponse<T>(Func<IRestResponse, T> selector, IRestResponse response, TaskCompletionSource<T> completionSource)
        {
            LogResponse(response);

            if (HasSucceeded(response))
            {
                completionSource.SetResult(selector(response));
            }
            else
            {
                LogError(response);
                completionSource.SetResult(default(T));
            }
        }

        private void ProcessResponse<T>(Func<IRestResponse<T>, T> selector, IRestResponse<T> response, TaskCompletionSource<T> completionSource)
            where T : class, new()
        {
            LogResponse(response);

            if (HasSucceeded(response))
            {
                completionSource.SetResult(selector(response));
            }
            else
            {
                LogError(response);
                completionSource.SetResult(null);
            }
        }

        private static string Encode(string parameterValue)
        {
            return HttpUtility.HtmlEncode(parameterValue);
        }

        private void LogRequest(IRestRequest request)
        {
            Logger.InfoFormat("HTTP {0} {1}/{2}", request.Method, _connection.Url, request.Resource);
            
            foreach (var parameter in request.Parameters)
            {
                Logger.DebugFormat("Request Parameter: {0} : {1}", 
                                                       parameter.Name, 
                                                       parameter.Value);
            }
        }

        private void LogResponse(IRestResponse response)
        {
            Logger.DebugFormat("HTTP Status {0} ({1}) ({2})", 
                                            response.StatusCode, 
                                            (int)response.StatusCode, 
                                            response.ResponseUri);

            foreach (var header in response.Headers)
            {
                Logger.DebugFormat("Response Header: {0} : {1}", 
                                                     header.Name, 
                                                     header.Value);
            }
        }

        private void LogError(IRestResponse response)
        {
            var errorMessage = string.Format("Error executing the request: {0}, Status code is {1}", 
                                                                           response.ErrorMessage, 
                                                                           response.StatusCode);
            RaiseAsyncOperationFailed(errorMessage);
            Logger.ErrorFormat(errorMessage, response.ErrorException);
        }

        private static bool HasSucceeded(IRestResponse response)
        {
            return SuccessCodes.Any(x => x == response.StatusCode);
        }

        private void RaiseAsyncOperationFailed(string errorMessage)
        {
            _eventAggregator.Publish(new AsyncOperationFailedEvent
            {
                Message = errorMessage
            });
        }

        private static IEnumerable<HttpStatusCode> SuccessCodes
        {
            get
            {
                yield return HttpStatusCode.OK;
                yield return HttpStatusCode.Accepted;
            }
        }
    }
}