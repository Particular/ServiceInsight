namespace ServiceInsight.ServiceControl
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net;
    using System.Runtime.Caching;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Anotar.Serilog;
    using Caliburn.Micro;
    using Framework;
    using RestSharp;
    using RestSharp.Authenticators;
    using RestSharp.Deserializers;
    using RestSharp.Extensions.MonoHttp;
    using Serilog;
    using ServiceInsight.ExtensionMethods;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.MessageDecoders;
    using ServiceInsight.Framework.Settings;
    using ServiceInsight.Models;
    using ServiceInsight.Saga;
    using ServiceInsight.Settings;

    public class DefaultServiceControl : IServiceControl
    {
        static ILogger anotarLogger = Log.ForContext<IServiceControl>();
        static byte[] byteOrderMarkUtf8 = Encoding.UTF8.GetPreamble();
        static IDeserializer jsonTruncatingDeserializer = CreateJsonDeserializer(truncateLargeLists: true);
        static IDeserializer jsonDeserializer = CreateJsonDeserializer(truncateLargeLists: false);

        const string ConversationEndpoint = "conversations/{0}";
        const string DefaultEndpointsEndpoint = "endpoints";
        const string EndpointMessagesEndpoint = "endpoints/{0}/messages/";
        const string RetryEndpoint = "errors/{0}/retry";
        const string MessagesEndpoint = "messages/";
        const string MessageBodyEndpoint = "messages/{0}/body";
        const string SagaEndpoint = "sagas/{0}";
        const int DefaultPageSize = 50;

        ServiceControlConnectionProvider connection;
        MemoryCache cache;
        IEventAggregator eventAggregator;
        ProfilerSettings settings;

        static DefaultServiceControl()
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => ApplicationConfiguration.SkipCertificateValidation;
        }

        public DefaultServiceControl(
            ServiceControlConnectionProvider connection,
            IEventAggregator eventAggregator,
            ISettingsProvider settingsProvider)
        {
            this.connection = connection;
            this.eventAggregator = eventAggregator;
            settings = settingsProvider.GetSettings<ProfilerSettings>();
            cache = new MemoryCache("ServiceControlReponses", new NameValueCollection(1) { { "cacheMemoryLimitMegabytes", settings.CacheSize.ToString() } });
        }

        public async Task<bool> IsAlive()
        {
            var version = await GetVersion().ConfigureAwait(false);

            if (version != null)
            {
                RaygunUtility.LastServiceControlVersion = version;
            }

            return version != null;
        }

        public async Task<string> GetVersion()
        {
            var request = new RestRequestWithCache(RestRequestWithCache.CacheStyle.Immutable);

            var header = await Execute(request, restResponse => restResponse.Headers.SingleOrDefault(x => x.Name == ServiceControlHeaders.ParticularVersion)).ConfigureAwait(false);

            return header == null ? null : header.Value.ToString();
        }

        Task<ServiceControlRootUrls> GetRootUrls()
        {
            var request = new RestRequestWithCache("/", RestRequestWithCache.CacheStyle.Immutable);

            return GetModel<ServiceControlRootUrls>(request);
        }

        public Task RetryMessage(string messageId, string instanceId)
        {
            var url = instanceId != null ?
                string.Format(RetryEndpoint + "?instance_id={1}", messageId, instanceId) :
                string.Format(RetryEndpoint, messageId);
            var request = new RestRequestWithCache(url, Method.POST);
            return Execute(request, _ => true);
        }

        public Uri CreateServiceInsightUri(StoredMessage message)
        {
            var connectionUri = new Uri(connection.Url);
            return new Uri(string.Format("si://{0}:{1}/api{2}", connectionUri.Host, connectionUri.Port, message.GetURIQuery()));
        }

        public async Task<SagaData> GetSagaById(Guid sagaId) => await GetModel<SagaData>(new RestRequestWithCache(string.Format(SagaEndpoint, sagaId), RestRequestWithCache.CacheStyle.IfNotModified), truncateLargeLists: true).ConfigureAwait(false) ?? new SagaData();

        public Task<PagedResult<StoredMessage>> GetAuditMessages(Endpoint endpoint = null, string searchQuery = null, string orderBy = null, bool ascending = false)
        {
            var request = CreateMessagesRequest(endpoint?.Name);

            AppendSystemMessages(request);
            AppendSearchQuery(request, searchQuery);
            AppendPaging(request);
            AppendOrdering(request, orderBy, ascending);

            return GetPagedResult<StoredMessage>(request);
        }

        public Task<PagedResult<StoredMessage>> GetAuditMessages(string link)
        {
            if (IsAbsoluteUrl(link))
            {
                var request = new RestRequestWithCache("", RestRequestWithCache.CacheStyle.IfNotModified);
                return GetPagedResult<StoredMessage>(request, link);
            }
            else
            {
                var request = new RestRequestWithCache(link, RestRequestWithCache.CacheStyle.IfNotModified);
                return GetPagedResult<StoredMessage>(request);
            }
        }

        public async Task<IEnumerable<StoredMessage>> GetConversationById(string conversationId)
        {
            var request = new RestRequestWithCache(string.Format(ConversationEndpoint, conversationId), RestRequestWithCache.CacheStyle.IfNotModified);
            var messages = await GetModel<List<StoredMessage>>(request).ConfigureAwait(false) ?? new List<StoredMessage>();
            return messages;
        }

        public async Task<IEnumerable<Endpoint>> GetEndpoints()
        {
            var rootUrls = await GetRootUrls().ConfigureAwait(false);

            var endpointsUrl = rootUrls?.KnownEndpointsUrl ?? rootUrls?.EndpointsUrl ?? DefaultEndpointsEndpoint;
            if (Uri.TryCreate(endpointsUrl, UriKind.Absolute, out var endpointsUri) &&
                Uri.TryCreate(connection.Url, UriKind.Absolute, out var connectionUri))
            {
                endpointsUrl = endpointsUri.PathAndQuery.Replace(connectionUri.PathAndQuery, string.Empty);
            }

            var request = new RestRequestWithCache(endpointsUrl, RestRequestWithCache.CacheStyle.IfNotModified);
            var endpoints = await GetModel<List<Endpoint>>(request).ConfigureAwait(false);
            return endpoints ?? new List<Endpoint>();
        }

        public async Task<IEnumerable<KeyValuePair<string, string>>> GetMessageData(SagaMessage message)
        {
            var request = new RestRequestWithCache(string.Format(MessageBodyEndpoint, message.MessageId), message.Status == MessageStatus.Successful ? RestRequestWithCache.CacheStyle.Immutable : RestRequestWithCache.CacheStyle.IfNotModified);

            var body = await Execute(request, response => response.Content, truncateLargeLists: true).ConfigureAwait(false);

            if (body == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            return body.StartsWith("<?xml") ? GetXmlData(body) : JsonPropertiesHelper.ProcessValues(body);
        }

        public async Task LoadBody(StoredMessage message)
        {
            var request = new RestRequestWithCache(message.BodyUrl, RestRequestWithCache.CacheStyle.Immutable);

            var baseUrl = message.BodyUrl;
            if (!baseUrl.StartsWith("http"))
            {
                baseUrl = null; // We use the default
            }

            message.Body = await Execute(request, response =>
            {
                var presentationBody = new PresentationBody();

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        presentationBody.Text = response.Content;
                        break;
                    case HttpStatusCode.NoContent:
                        presentationBody.Hint = PresentationHint.NoContent;
                        presentationBody.Text = "Body was too large and not stored. Edit ServiceControl/MaxBodySizeToStore to be larger in the ServiceControl configuration.";
                        break;
                }

                return presentationBody;
            }, baseUrl).ConfigureAwait(false);
        }

        void AppendSystemMessages(IRestRequest request)
        {
            request.AddParameter("include_system_messages", settings.DisplaySystemMessages);
        }

        static void AppendOrdering(IRestRequest request, string orderBy, bool ascending)
        {
            if (orderBy == null)
            {
                return;
            }

            request.AddParameter("sort", orderBy, ParameterType.GetOrPost);
            request.AddParameter("direction", ascending ? "asc" : "desc", ParameterType.GetOrPost);
        }

        static void AppendPaging(IRestRequest request)
        {
            request.AddParameter("per_page", DefaultPageSize, ParameterType.GetOrPost);
        }

        void AppendSearchQuery(IRestRequest request, string searchQuery)
        {
            if (searchQuery == null)
            {
                return;
            }

            request.Resource += "search";
            request.AddParameter("q", searchQuery, ParameterType.GetOrPost);
        }

        IRestClient CreateClient(string baseUrl = null, bool truncateLargeLists = false)
        {
            var client = new RestClient(baseUrl ?? connection.Url)
            {
                Authenticator = new NtlmAuthenticator()
            };

            client.ClearHandlers();
            client.AddJsonDeserializer(truncateLargeLists ? jsonTruncatingDeserializer : jsonDeserializer);
            client.AddXmlDeserializer(new XmlDeserializer());
            client.AddDefaultHeader("Accept-Encoding", "gzip,deflate");

            return client;
        }

        static IDeserializer CreateJsonDeserializer(bool truncateLargeLists = false)
        {
            return new JsonMessageDeserializer
            {
                DateFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK",
                TruncateLargeLists = truncateLargeLists
            };
        }

        static RestRequestWithCache CreateMessagesRequest(string endpointName = null) => endpointName != null
    ? new RestRequestWithCache(string.Format(EndpointMessagesEndpoint, endpointName), RestRequestWithCache.CacheStyle.IfNotModified)
    : new RestRequestWithCache(MessagesEndpoint, RestRequestWithCache.CacheStyle.IfNotModified);

        Task<PagedResult<T>> GetPagedResult<T>(RestRequestWithCache request, string url = null) where T : class, new()
        {
            var result = Execute<PagedResult<T>, List<T>>(request, response =>
            {
                string first = null, prev = null, next = null, last = null;
                var links = (string)response.Headers.FirstOrDefault(header => header.Name == ServiceControlHeaders.Link)?.Value;
                if (links != null)
                {
                    var linksByRel = linkExpression.Matches(links)
                        .Cast<Match>()
                        .ToDictionary(match => match.Groups[2].Value, match => match.Groups[1].Value);

                    linksByRel.TryGetValue("first", out first);
                    linksByRel.TryGetValue("prev", out prev);
                    linksByRel.TryGetValue("next", out next);
                    linksByRel.TryGetValue("last", out last);
                }

                var requestQueryParameters = HttpUtility.ParseQueryString(url ?? request.Resource);

                var pageSize = DefaultPageSize;
                var perPage = requestQueryParameters["per_page"];
                if (perPage != null)
                {
                    pageSize = int.Parse(perPage);
                }

                var pageSizeText = (string)response.Headers.FirstOrDefault(header => header.Name == ServiceControlHeaders.PageSize)?.Value;
                if (pageSizeText != null)
                {
                    pageSize = int.Parse(pageSizeText);
                }

                var currentPage = 1;
                var page = requestQueryParameters["page"];
                if (page != null)
                {
                    currentPage = int.Parse(page);
                }

                return new PagedResult<T>
                {
                    CurrentPage = currentPage,
                    Result = response.Data,
                    NextLink = next,
                    PrevLink = prev,
                    LastLink = last,
                    FirstLink = first,
                    TotalCount = int.Parse(response.Headers.First(x => x.Name == ServiceControlHeaders.TotalCount).Value.ToString()),
                    PageSize = pageSize
                };
            },
            url);

            return result;
        }

        Task<T> GetModel<T>(RestRequestWithCache request, bool truncateLargeLists = false)
where T : class, new() => Execute<T, T>(request, response => response.Data, truncateLargeLists: truncateLargeLists);

        async Task<T> Execute<T>(RestRequestWithCache request, Func<IRestResponse, T> selector, string baseUrl = null, bool truncateLargeLists = false)
        {
            var cacheStyle = request.CacheSyle;
            var restClient = CreateClient(baseUrl, truncateLargeLists);

            switch (cacheStyle)
            {
                case RestRequestWithCache.CacheStyle.None:
                    break;
                case RestRequestWithCache.CacheStyle.Immutable:
                    var item = cache.Get(CacheKey<T>(restClient, request));

                    if (item != null)
                    {
                        return (T)item;
                    }

                    break;
                case RestRequestWithCache.CacheStyle.IfNotModified:
                    var obj = cache.Get(CacheKey<T>(restClient, request));

                    if (obj != null)
                    {
                        var tuple = (Tuple<string, T>)obj;
                        request.AddHeader("If-None-Match", tuple.Item1);
                    }

                    break;
            }

            LogRequest(request);

            var response = await restClient.ExecuteTaskAsync(request).ConfigureAwait(false);

            CleanResponse(response);

            var data = default(T);

            switch (cacheStyle)
            {
                case RestRequestWithCache.CacheStyle.Immutable:
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        data = ProcessResponse(selector, response);
                        CacheData(request, restClient, data);
                    }
                    // https://github.com/Particular/FeatureDevelopment/issues/296
                    else if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        data = ProcessResponse(selector, response);
                        CacheData(request, restClient, data);
                    }

                    break;

                case RestRequestWithCache.CacheStyle.IfNotModified:
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            data = ProcessResponse(selector, response);
                            var etag = response.Headers.FirstOrDefault(h => h.Name == "ETag");
                            if (etag != null)
                            {
                                CacheData(request, restClient, Tuple.Create(etag.Value.ToString(), data));
                            }
                            break;
                        case HttpStatusCode.NotModified:
                            LogResponse(response);

                            var obj = cache.Get(CacheKey<T>(restClient, request));

                            if (obj != null)
                            {
                                var tuple = (Tuple<string, T>)obj;
                                data = tuple.Item2;
                            }
                            break;
                    }
                    break;
                default:
                    data = ProcessResponse(selector, response);
                    break;
            }

            return data;
        }

        async Task<T> Execute<T, T2>(RestRequestWithCache request, Func<IRestResponse<T2>, T> selector, string url = null, bool truncateLargeLists = false)
            where T : class, new()
            where T2 : class, new()
        {
            var cacheStyle = request.CacheSyle;
            var restClient = CreateClient(url, truncateLargeLists);

            switch (cacheStyle)
            {
                case RestRequestWithCache.CacheStyle.None:
                    break;
                case RestRequestWithCache.CacheStyle.Immutable:
                    var item = cache.Get(CacheKey<T>(restClient, request));

                    if (item != null)
                    {
                        return (T)item;
                    }

                    break;
                case RestRequestWithCache.CacheStyle.IfNotModified:
                    var obj = cache.Get(CacheKey<T>(restClient, request));

                    if (obj != null)
                    {
                        var tuple = (Tuple<string, T>)obj;
                        request.AddHeader("If-None-Match", tuple.Item1);
                    }

                    break;
            }

            LogRequest(request);

            var response = await restClient.ExecuteTaskAsync<T2>(request).ConfigureAwait(false);

            CleanResponse(response);

            var data = default(T);

            switch (cacheStyle)
            {
                case RestRequestWithCache.CacheStyle.Immutable:
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        data = ProcessResponse(selector, response);
                        CacheData(request, restClient, data);
                    }
                    break;

                case RestRequestWithCache.CacheStyle.IfNotModified:
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            data = ProcessResponse(selector, response);
                            var etag = response.Headers.FirstOrDefault(h => h.Name == "ETag");
                            if (etag != null)
                            {
                                CacheData(request, restClient, Tuple.Create(etag.Value.ToString(), data));
                            }
                            break;
                        case HttpStatusCode.NotModified:
                            LogResponse(response);

                            var obj = cache.Get(CacheKey<T>(restClient, request));

                            if (obj != null)
                            {
                                var tuple = (Tuple<string, T>)obj;
                                data = tuple.Item2;
                            }
                            break;
                    }
                    break;
                default:
                    data = ProcessResponse(selector, response);
                    break;
            }

            return data;
        }

        void CacheData(RestRequestWithCache request, IRestClient restClient, object data)
        {
            if (data != null)
            {
                cache.Set(CacheKey(restClient, request, data.GetType()), data, new CacheItemPolicy());
            }
        }

        static string CacheKey<T>(IRestClient restClient, IRestRequest request) => CacheKey(restClient, request, typeof(T));

        static string CacheKey(IRestClient restClient, IRestRequest request, Type t) => restClient.BuildUri(request).AbsoluteUri + "-" + t;

        T ProcessResponse<T>(Func<IRestResponse, T> selector, IRestResponse response)
        {
            if (HasSucceeded(response))
            {
                LogResponse(response);
                return selector(response);
            }

            LogError(response);
            return default(T);
        }

        T ProcessResponse<T, T2>(Func<IRestResponse<T2>, T> selector, IRestResponse<T2> response)
        {
            if (HasSucceeded(response))
            {
                LogResponse(response);
                return selector(response);
            }

            LogError(response);
            return default(T);
        }

        IEnumerable<KeyValuePair<string, string>> GetXmlData(string bodyString)
        {
            try
            {
                var xml = XDocument.Parse(bodyString);
                if (xml.Root != null)
                {
                    // < v5 Messages were wrapped in a root "Messages" node
                    // v5+ A message is the root node
                    var root = xml.Root.Name.LocalName == "Messages" ? xml.Root.Elements().FirstOrDefault() : xml.Root;
                    if (root != null)
                    {
                        return root.Elements()
                                   .Select(n => new KeyValuePair<string, string>(n.Name.LocalName, n.Value));
                    }
                }
            }
            catch (XmlException ex)
            {
                LogTo.Error(ex, "Error parsing message data.");
            }
            return new List<KeyValuePair<string, string>>();
        }

        static void CleanResponse(IRestResponse response)
        {
            if (response.RawBytes.StartsWith(byteOrderMarkUtf8))
            {
                var strippedBytes = response.RawBytes
                                            .Skip(byteOrderMarkUtf8.Length)
                                            .ToArray();

                response.Content = new UTF8Encoding(false).GetString(strippedBytes);
            }
        }

        void LogRequest(RestRequestWithCache request)
        {
            var resource = request.Resource != null ? request.Resource.TrimStart('/') : string.Empty;
            var url = connection.Url != null ? connection.Url.TrimEnd('/') : string.Empty;

            LogTo.Information("HTTP {Method} {url:l}/{resource:l} ({CacheSyle})", request.Method, url, resource, request.CacheSyle);

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

            LogTo.Debug("HTTP Status {code} ({uri}) Headers: {headers}", code, uri, response.Headers);
        }

        void LogError(IRestResponse response)
        {
            var exception = response != null ? response.ErrorException : null;
            var errorMessage = response != null ? string.Format("Error executing the request: {0}, Status code is {1}", response.ErrorMessage, response.StatusCode) : "No response was received.";

            eventAggregator.PublishOnUIThread(new AsyncOperationFailed(errorMessage));
            LogTo.Error(exception, errorMessage);
        }

        static bool IsAbsoluteUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }

        static bool HasSucceeded(IRestResponse response) => successCodes.Any(x => response != null && x == response.StatusCode && response.ErrorException == null);

        static IEnumerable<HttpStatusCode> successCodes = new[] { HttpStatusCode.OK, HttpStatusCode.Accepted, HttpStatusCode.NotModified, HttpStatusCode.NoContent };
        static Regex linkExpression = new Regex(@"<([^>]+)>;\s?rel=""(\w+)"",?\s?", RegexOptions.Compiled);
    }

    public enum PresentationHint
    {
        Standard,
        NoContent
    }

    public class PresentationBody
    {
        PresentationHint hint = PresentationHint.Standard;

        public string Text { get; set; }

        public PresentationHint Hint
        {
            get { return hint; }
            set { hint = value; }
        }
    }
}