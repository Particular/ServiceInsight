using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Caliburn.PresentationFramework.ApplicationModel;
using log4net;
using NServiceBus.Profiler.Desktop.Core.MessageDecoders;
using NServiceBus.Profiler.Desktop.Core.Settings;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.Desktop.Settings;
using RestSharp;
using RestSharp.Contrib;

namespace NServiceBus.Profiler.Desktop.ServiceControl
{
    public class DefaultServiceControl : IServiceControl
    {
        public class ServiceControlHeaders
        {
            public const string ParticularVersion = "X-Particular-Version";
            public const string TotalCount = "Total-Count";
        }

        private readonly IServiceControlConnectionProvider _connection;
        private readonly IEventAggregator _eventAggregator;
        private readonly ProfilerSettings _settings;
        private readonly ILog _logger = LogManager.GetLogger(typeof(IServiceControl));

        public DefaultServiceControl(
            IServiceControlConnectionProvider connection, 
            IEventAggregator eventAggregator,
            ISettingsProvider settingsProvider)
        {
            _connection = connection;
            _eventAggregator = eventAggregator;
            _settings = settingsProvider.GetSettings<ProfilerSettings>();
        }


        public async Task<PagedResult<StoredMessage>> Search(string searchQuery, int pageIndex = 1, string orderBy = null, bool ascending = false)
        {
            var request = new RestRequest(CreateBaseUrl());
            
            AppendSystemMessages(request);
            AppendSearchQuery(request, searchQuery);
            AppendPaging(request, pageIndex);
            AppendOrdering(request, orderBy, ascending);

            var result = await GetPagedResult<StoredMessage>(request);
            result.CurrentPage = pageIndex;

            return result;
        }

        public async Task<PagedResult<StoredMessage>> GetAuditMessages(Endpoint endpoint, string searchQuery = null, int pageIndex = 1, string orderBy = null, bool ascending = false)
        {
            var request = new RestRequest(CreateBaseUrl(endpoint.Name));

            AppendSystemMessages(request);
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
                                  restResponse.Headers.First(x => x.Name == ServiceControlHeaders.ParticularVersion).Value.ToString(),
                                  response, completionSource));

            return await completionSource.Task;
        }

        public async Task<bool> RetryMessage(string messageId)
        {
            var url = string.Format("errors/{0}/retry", messageId);
            var request = new RestRequest(url, Method.POST);
            var response = await ExecuteAsync(request, HasSucceeded);

            return response;
        }

        public async Task<string> GetBody(string bodyUrl)
        {
            IRestClient client;

            if (bodyUrl.StartsWith("http"))
            {
                client = CreateClient(bodyUrl);
            }
            else
            {
                client = CreateClient();
            }

            return await ExecuteAsync(client, new RestRequest(bodyUrl, Method.GET), r => HasSucceeded(r) ? r.Content : string.Empty);
        }

        public Uri GetUri(StoredMessage message)
        {
            var connectionUri = new Uri(_connection.Url);
            return new Uri(string.Format("si://{0}:{1}/api{2}", connectionUri.Host, connectionUri.Port, message.GetURIQuery()));
        }

        private void AppendSystemMessages(IRestRequest request)
        {
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
            return CreateClient(_connection.Url);
        }

        private IRestClient CreateClient(string url)
        {
            var client = new RestClient(url);
            var deserializer = new JsonMessageDeserializer();
            client.ClearHandlers();
            client.AddHandler("application/json", deserializer);
            client.AddHandler("text/json", deserializer);
            client.AddHandler("text/x-json", deserializer);
            client.AddHandler("text/javascript", deserializer);
            client.AddDefaultHeader("Accept-Encoding", "gzip,deflate");
            return client;
        }

        private static string CreateBaseUrl(string endpointName = null)
        {
            return endpointName != null ? string.Format("endpoints/{0}/messages/", endpointName) : "messages/";
        }

        private Task<PagedResult<T>> GetPagedResult<T>(IRestRequest request) where T : class, new()
        {
            LogRequest(request);

            var completionSource = new TaskCompletionSource<PagedResult<T>>();
            var client = CreateClient();

            client.ExecuteAsync<List<T>>(request, response =>
            {
                if (HasSucceeded(response))
                {
                    LogResponse(response);

                    completionSource.SetResult(new PagedResult<T>
                    {
                        Result = response.Data,
                        TotalCount = int.Parse(response.Headers.First(x => x.Name == ServiceControlHeaders.TotalCount).Value.ToString())
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

        private Task<T> ExecuteAsync<T>(IRestRequest request, Func<IRestResponse, T> selector)
        {
            return ExecuteAsync(CreateClient(), request, selector);
        }

        private Task<T> ExecuteAsync<T>(IRestClient client, IRestRequest request, Func<IRestResponse, T> selector)
        {
            LogRequest(request);

            var completionSource = new TaskCompletionSource<T>();
            
            client.ExecuteAsync(request, response => ProcessResponse(selector, response, completionSource));
            
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
            if (HasSucceeded(response))
            {
                LogResponse(response);
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
            if (HasSucceeded(response))
            {
                LogResponse(response);
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
            return HttpUtility.UrlEncode(parameterValue);
        }

        private void LogRequest(IRestRequest request)
        {
            var resource = request.Resource != null ? request.Resource.TrimStart('/') : string.Empty;
            var url = _connection.Url != null ? _connection.Url.TrimEnd('/') : string.Empty;

            _logger.InfoFormat("HTTP {0} {1}/{2}", request.Method, url, resource);
            
            foreach (var parameter in request.Parameters)
            {
                _logger.DebugFormat("Request Parameter: {0} : {1}", 
                                                       parameter.Name, 
                                                       parameter.Value);
            }
        }

        private void LogResponse(IRestResponse response)
        {
            var code = response.StatusCode;
            var uri = response.ResponseUri;

            _logger.DebugFormat("HTTP Status {0} ({1}) ({2})", code, (int)code, uri);

            foreach (var header in response.Headers)
            {
                _logger.DebugFormat("Response Header: {0} : {1}", 
                                                     header.Name, 
                                                     header.Value);
            }
        }

        private void LogError(IRestResponse response)
        {
            var exception = response != null ? response.ErrorException : null;
            var errorMessage = response != null ? string.Format("Error executing the request: {0}, Status code is {1}", response.ErrorMessage, response.StatusCode) : "No response was received.";
            
            RaiseAsyncOperationFailed(errorMessage);
            _logger.ErrorFormat(errorMessage, exception);
        }

        private static bool HasSucceeded(IRestResponse response)
        {
            return SuccessCodes.Any(x => response != null && x == response.StatusCode && response.ErrorException == null);
        }

        private void RaiseAsyncOperationFailed(string errorMessage)
        {
            _eventAggregator.Publish(new AsyncOperationFailed(errorMessage));
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