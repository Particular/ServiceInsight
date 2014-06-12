namespace Particular.ServiceInsight.Desktop.ServiceControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Anotar.Serilog;
    using Caliburn.Micro;
    using Core.MessageDecoders;
    using Core.Settings;
    using Events;
    using Models;
    using RestSharp;
    using RestSharp.Contrib;
    using Saga;
    using Settings;

    public class DefaultServiceControl : IServiceControl
    {
        ServiceControlConnectionProvider connection;
        IEventAggregator eventAggregator;
        ProfilerSettings settings;

        public DefaultServiceControl(
            ServiceControlConnectionProvider connection,
            IEventAggregator eventAggregator,
            ISettingsProvider settingsProvider)
        {
            this.connection = connection;
            this.eventAggregator = eventAggregator;
            settings = settingsProvider.GetSettings<ProfilerSettings>();
        }

        public PagedResult<StoredMessage> Search(string searchQuery, int pageIndex = 1, string orderBy = null, bool ascending = false)
        {
            var request = new RestRequest(CreateBaseUrl());

            AppendSystemMessages(request);
            AppendSearchQuery(request, searchQuery);
            AppendPaging(request, pageIndex);
            AppendOrdering(request, orderBy, ascending);

            var result = GetPagedResult<StoredMessage>(request);
            result.CurrentPage = pageIndex;

            return result;
        }

        public PagedResult<StoredMessage> GetAuditMessages(Endpoint endpoint, string searchQuery = null, int pageIndex = 1, string orderBy = null, bool ascending = false)
        {
            var request = new RestRequest(CreateBaseUrl(endpoint.Name));

            AppendSystemMessages(request);
            AppendSearchQuery(request, searchQuery);
            AppendPaging(request, pageIndex);
            AppendOrdering(request, orderBy, ascending);

            var result = GetPagedResult<StoredMessage>(request);
            result.CurrentPage = pageIndex;

            return result;
        }

        public List<StoredMessage> GetConversationById(string conversationId)
        {
            var request = new RestRequest(string.Format("conversations/{0}", conversationId));
            var messages = GetModelAsync<List<StoredMessage>>(request) ?? new List<StoredMessage>();

            return messages;
        }

        public List<Endpoint> GetEndpoints()
        {
            var request = new RestRequest("endpoints");
            var messages = GetModelAsync<List<Endpoint>>(request);

            return messages ?? new List<Endpoint>();
        }

        public bool IsAlive()
        {
            return GetVersion() != null;
        }

        public string GetVersion()
        {
            var request = new RestRequest();

            LogRequest(request);

            var response = CreateClient().Execute(request);
            return ProcessResponse(restResponse => restResponse.Headers.First(x => x.Name == ServiceControlHeaders.ParticularVersion).Value.ToString(), response);
        }

        public bool RetryMessage(string messageId)
        {
            var url = string.Format("errors/{0}/retry", messageId);
            var request = new RestRequest(url, Method.POST);
            var response = ExecuteAsync(request, HasSucceeded);

            return response;
        }

        public string GetBody(string bodyUrl)
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

            return ExecuteAsync(client, new RestRequest(bodyUrl, Method.GET), r => HasSucceeded(r) ? r.Content : string.Empty);
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

        PagedResult<T> GetPagedResult<T>(IRestRequest request) where T : class, new()
        {
            LogRequest(request);

            var response = CreateClient().Execute<List<T>>(request);

            if (HasSucceeded(response))
            {
                LogResponse(response);
                return new PagedResult<T>
                {
                    Result = response.Data,
                    TotalCount = int.Parse(response.Headers.First(x => x.Name == ServiceControlHeaders.TotalCount).Value.ToString())
                };
            }
            else
            {
                LogError(response);
                return new PagedResult<T>();
            }
        }

        public bool HasSagaChanged(string sagaId)
        {
            return HasChanged(CreateSagaRequest(sagaId));
        }

        public SagaData GetSagaById(string sagaId)
        {
            return GetModelAsync<SagaData>(CreateSagaRequest(sagaId)) ?? new SagaData();
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
                        string.Equals(((RestSharp.Parameter)c.Value).Value, etag.Value));
                }
                finally
                {
                    request.Method = method;
                }
            }

            return true;
        }

        T GetModelAsync<T>(IRestRequest request)
            where T : class, new()
        {
            return ExecuteAsync<T>(request, response => { CacheResponse(response); return response.Data; });
        }

        T ExecuteAsync<T>(IRestRequest request, Func<IRestResponse, T> selector)
        {
            return ExecuteAsync(CreateClient(), request, selector);
        }

        T ExecuteAsync<T>(IRestClient client, IRestRequest request, Func<IRestResponse, T> selector)
        {
            LogRequest(request);

            var response = client.Execute(request);
            return ProcessResponse(selector, response);
        }

        T ExecuteAsync<T>(IRestRequest request, Func<IRestResponse<T>, T> selector)
            where T : class, new()
        {
            LogRequest(request);

            var response = CreateClient().Execute<T>(request);
            return ProcessResponse(selector, response);
        }

        static void CacheResponse(IRestResponse response)
        {
            if (response.Request.Resource != null && response.Headers.Any(h => h.Name == "ETag"))
            {
                System.Runtime.Caching.MemoryCache.Default.Add(new System.Runtime.Caching.CacheItem(response.Request.Resource, response.Headers.FirstOrDefault(h => h.Name == "ETag")), new System.Runtime.Caching.CacheItemPolicy());
            }
        }

        T ProcessResponse<T>(Func<IRestResponse, T> selector, IRestResponse response)
        {
            if (HasSucceeded(response))
            {
                LogResponse(response);
                return selector(response);
            }
            else
            {
                LogError(response);
                return default(T);
            }
        }

        T ProcessResponse<T>(Func<IRestResponse<T>, T> selector, IRestResponse<T> response)
        {
            if (HasSucceeded(response))
            {
                LogResponse(response);
                return selector(response);
            }
            else
            {
                LogError(response);
                return default(T);
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

            LogTo.Information("HTTP {Method} {url:l}/{resource:l}", request.Method, url, resource);

            foreach (var parameter in request.Parameters)
            {
                LogTo.Debug("Request Parameter: {Name} : {Value}",
                                                       parameter.Name,
                                                       parameter.Value);
            }
        }

        void LogResponse(IRestResponse response)
        {
            var code = response.StatusCode;
            var uri = response.ResponseUri;

            LogTo.Debug("HTTP Status {code} ({uri})", code, uri);

            foreach (var header in response.Headers)
            {
                LogTo.Debug("Response Header: {Name} : {Value}",
                                                     header.Name,
                                                     header.Value);
            }
        }

        void LogError(IRestResponse response)
        {
            var exception = response != null ? response.ErrorException : null;
            var errorMessage = response != null ? string.Format("Error executing the request: {0}, Status code is {1}", response.ErrorMessage, response.StatusCode) : "No response was received.";

            RaiseAsyncOperationFailed(errorMessage);
            LogTo.Error(exception, errorMessage);
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