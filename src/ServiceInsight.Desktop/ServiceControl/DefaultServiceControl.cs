namespace Particular.ServiceInsight.Desktop.ServiceControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Core.MessageDecoders;
    using Core.Settings;
    using Events;
    using Models;
    using RestSharp;
    using RestSharp.Contrib;
    using Saga;
    using Settings;
    using L4NILog = log4net.ILog;
    using L4NLogManager = log4net.LogManager;

    public class DefaultServiceControl
    {
        public class ServiceControlHeaders
        {
            public const string ParticularVersion = "X-Particular-Version";
            public const string TotalCount = "Total-Count";
        }

        ServiceControlConnectionProvider connection;
        IEventAggregator eventAggregator;
        ProfilerSettings settings;
        L4NILog logger = L4NLogManager.GetLogger(typeof(DefaultServiceControl));

        public DefaultServiceControl(
            ServiceControlConnectionProvider connection,
            IEventAggregator eventAggregator,
            ISettingsProvider settingsProvider)
        {
            this.connection = connection;
            this.eventAggregator = eventAggregator;
            settings = settingsProvider.GetSettings<ProfilerSettings>();
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
            var connectionUri = new Uri(connection.Url);
            return new Uri(string.Format("si://{0}:{1}/api{2}", connectionUri.Host, connectionUri.Port, message.GetURIQuery()));
        }

        void AppendSystemMessages(IRestRequest request)
        {
            request.AddParameter("include_system_messages", settings.DisplaySystemMessages);
        }

        void AppendOrdering(IRestRequest request, string orderBy, bool ascending)
        {
            if (orderBy == null) return;
            request.AddParameter("sort", orderBy, ParameterType.GetOrPost);
            request.AddParameter("direction", GetSortDirection(ascending), ParameterType.GetOrPost);
        }

        void AppendPaging(IRestRequest request, int pageIndex)
        {
            request.AddParameter("page", pageIndex, ParameterType.GetOrPost);
        }

        void AppendSearchQuery(IRestRequest request, string searchQuery)
        {
            if (searchQuery == null) return;
            request.Resource += string.Format("search/{0}", Encode(searchQuery));
        }

        string GetSortDirection(bool ascending)
        {
            return ascending ? "asc" : "desc";
        }

        IRestClient CreateClient()
        {
            return CreateClient(connection.Url);
        }

        IRestClient CreateClient(string url)
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

        static string CreateBaseUrl(string endpointName = null)
        {
            return endpointName != null ? string.Format("endpoints/{0}/messages/", endpointName) : "messages/";
        }

        Task<PagedResult<T>> GetPagedResult<T>(IRestRequest request) where T : class, new()
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

        public bool HasSagaChanged(string sagaId)
        {
            return HasChanged(CreateSagaRequest(sagaId));
        }

        public async Task<SagaData> GetSagaById(string sagaId)
        {
            return await GetModelAsync<SagaData>(CreateSagaRequest(sagaId)) ?? new SagaData();
        }

        static RestRequest CreateSagaRequest(string sagaId)
        {
            return new RestRequest(string.Format("sagas/{0}", sagaId));
        }

        bool HasChanged(IRestRequest request)
        {
            if (System.Runtime.Caching.MemoryCache.Default.Any(c => c.Key == request.Resource))
            {
                var method = request.Method;
                try
                {
                    request.Method = Method.HEAD;
                    var response = CreateClient().Execute(request);

                    var etag = response.Headers.FirstOrDefault(h => h.Name == "ETag");
                    if (etag == null) return true;
                    return !System.Runtime.Caching.MemoryCache.Default.Any(c => c.Key == request.Resource &&
                        string.Equals(((Parameter)c.Value).Value, etag.Value));
                }
                finally
                {
                    request.Method = method;
                }
            }

            return true;
        }

        Task<T> GetModelAsync<T>(IRestRequest request)
            where T : class, new()
        {
            return ExecuteAsync<T>(request, response => { CacheResponse(response); return response.Data; });
        }

        Task<T> ExecuteAsync<T>(IRestRequest request, Func<IRestResponse, T> selector)
        {
            return ExecuteAsync(CreateClient(), request, selector);
        }

        Task<T> ExecuteAsync<T>(IRestClient client, IRestRequest request, Func<IRestResponse, T> selector)
        {
            LogRequest(request);

            var completionSource = new TaskCompletionSource<T>();

            client.ExecuteAsync(request, response => ProcessResponse(selector, response, completionSource));

            return completionSource.Task;
        }

        Task<T> ExecuteAsync<T>(IRestRequest request, Func<IRestResponse<T>, T> selector)
            where T : class, new()
        {
            LogRequest(request);

            var completionSource = new TaskCompletionSource<T>();
            CreateClient().ExecuteAsync<T>(request, response => ProcessResponse(selector, response, completionSource));
            return completionSource.Task;
        }

        void ProcessResponse<T>(Func<IRestResponse, T> selector, IRestResponse response, TaskCompletionSource<T> completionSource)
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

        static void CacheResponse(IRestResponse response)
        {
            if (response.Request.Resource != null && response.Headers.Any(h => h.Name == "ETag"))
            {
                System.Runtime.Caching.MemoryCache.Default.Add(new System.Runtime.Caching.CacheItem(response.Request.Resource, response.Headers.FirstOrDefault(h => h.Name == "ETag")), new System.Runtime.Caching.CacheItemPolicy());
            }
        }

        void ProcessResponse<T>(Func<IRestResponse<T>, T> selector, IRestResponse<T> response, TaskCompletionSource<T> completionSource)
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

        static string Encode(string parameterValue)
        {
            return HttpUtility.UrlEncode(parameterValue);
        }

        void LogRequest(IRestRequest request)
        {
            var resource = request.Resource != null ? request.Resource.TrimStart('/') : string.Empty;
            var url = connection.Url != null ? connection.Url.TrimEnd('/') : string.Empty;

            logger.InfoFormat("HTTP {0} {1}/{2}", request.Method, url, resource);

            foreach (var parameter in request.Parameters)
            {
                logger.DebugFormat("Request Parameter: {0} : {1}",
                                                       parameter.Name,
                                                       parameter.Value);
            }
        }

        void LogResponse(IRestResponse response)
        {
            var code = response.StatusCode;
            var uri = response.ResponseUri;

            logger.DebugFormat("HTTP Status {0} ({1}) ({2})", code, (int)code, uri);

            foreach (var header in response.Headers)
            {
                logger.DebugFormat("Response Header: {0} : {1}",
                                                     header.Name,
                                                     header.Value);
            }
        }

        void LogError(IRestResponse response)
        {
            var exception = response != null ? response.ErrorException : null;
            var errorMessage = response != null ? string.Format("Error executing the request: {0}, Status code is {1}", response.ErrorMessage, response.StatusCode) : "No response was received.";

            RaiseAsyncOperationFailed(errorMessage);
            logger.ErrorFormat(errorMessage, exception);
        }

        static bool HasSucceeded(IRestResponse response)
        {
            return SuccessCodes.Any(x => response != null && x == response.StatusCode && response.ErrorException == null);
        }

        void RaiseAsyncOperationFailed(string errorMessage)
        {
            eventAggregator.Publish(new AsyncOperationFailed(errorMessage));
        }

        static IEnumerable<HttpStatusCode> SuccessCodes
        {
            get
            {
                yield return HttpStatusCode.OK;
                yield return HttpStatusCode.Accepted;
            }
        }
    }
}