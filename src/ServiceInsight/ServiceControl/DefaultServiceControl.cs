namespace ServiceInsight.ServiceControl
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net;
    using System.Runtime.Caching;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Anotar.Serilog;
    using Caliburn.Micro;
    using Framework;
    using RestSharp;
    using RestSharp.Deserializers;
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

        const string ConversationEndpoint = "conversations/{0}";
        const string EndpointsEndpoint = "endpoints";
        const string EndpointMessagesEndpoint = "endpoints/{0}/messages/";
        const string RetryEndpoint = "errors/{0}/retry";
        const string MessagesEndpoint = "messages/";
        const string MessageBodyEndpoint = "messages/{0}/body";
        const string SagaEndpoint = "sagas/{0}";

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

        public bool IsAlive()
        {
            var version = GetVersion();

            if (version != null)
            {
                RaygunUtility.LastServiceControlVersion = version;
            }

            return version != null;
        }

        public string GetVersion()
        {
            var request = new RestRequestWithCache(RestRequestWithCache.CacheStyle.Immutable);

            var header = Execute(request, restResponse => restResponse.Headers.SingleOrDefault(x => x.Name == ServiceControlHeaders.ParticularVersion));

            return header == null ? null : header.Value.ToString();
        }

        public void RetryMessage(string messageId, string instanceId)
        {
            var url = instanceId != null ?
                string.Format(RetryEndpoint + "?instance_id={1}", messageId, instanceId) :
                string.Format(RetryEndpoint, messageId);
            var request = new RestRequestWithCache(url, Method.POST);
            Execute(request, _ => true);
        }

        public Uri CreateServiceInsightUri(StoredMessage message)
        {
            var connectionUri = new Uri(connection.Url);
            return new Uri(string.Format("si://{0}:{1}/api{2}", connectionUri.Host, connectionUri.Port, message.GetURIQuery()));
        }

        public SagaData GetSagaById(Guid sagaId) => GetModel<SagaData>(new RestRequestWithCache(string.Format(SagaEndpoint, sagaId), RestRequestWithCache.CacheStyle.IfNotModified)) ?? new SagaData();

        public PagedResult<StoredMessage> Search(string searchQuery, int pageIndex = 1, string orderBy = null, bool ascending = false)
        {
            var request = CreateMessagesRequest();

            AppendSystemMessages(request);
            AppendSearchQuery(request, searchQuery);
            AppendPaging(request, pageIndex);
            AppendOrdering(request, orderBy, ascending);

            var result = GetPagedResult<StoredMessage>(request);
            if (result != null)
            {
                result.CurrentPage = pageIndex;
            }
            return result;
        }

        public PagedResult<StoredMessage> GetAuditMessages(Endpoint endpoint, string searchQuery = null, int pageIndex = 1, string orderBy = null, bool ascending = false)
        {
            var request = CreateMessagesRequest(endpoint.Name);

            AppendSystemMessages(request);
            AppendSearchQuery(request, searchQuery);
            AppendPaging(request, pageIndex);
            AppendOrdering(request, orderBy, ascending);

            var result = GetPagedResult<StoredMessage>(request);
            if (result != null)
            {
                result.CurrentPage = pageIndex;
            }
            return result;
        }

        public IEnumerable<StoredMessage> GetConversationById(string conversationId)
        {
            var request = new RestRequestWithCache(string.Format(ConversationEndpoint, conversationId), RestRequestWithCache.CacheStyle.IfNotModified);
            var messages = GetModel<List<StoredMessage>>(request) ?? new List<StoredMessage>();

            return messages;
        }

        public IEnumerable<Endpoint> GetEndpoints()
        {
            var request = new RestRequestWithCache(EndpointsEndpoint, RestRequestWithCache.CacheStyle.IfNotModified);
            var messages = GetModel<List<Endpoint>>(request);

            return messages ?? new List<Endpoint>();
        }

        public IEnumerable<KeyValuePair<string, string>> GetMessageData(SagaMessage message)
        {
            var request = new RestRequestWithCache(string.Format(MessageBodyEndpoint, message.MessageId), message.Status == MessageStatus.Successful ? RestRequestWithCache.CacheStyle.Immutable : RestRequestWithCache.CacheStyle.IfNotModified);

            var body = Execute(request, response => response.Content);

            if (body == null)
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            return body.StartsWith("<?xml") ? GetXmlData(body) : JsonPropertiesHelper.ProcessValues(body);
        }

        public void LoadBody(StoredMessage message)
        {
            var request = new RestRequestWithCache(message.BodyUrl, RestRequestWithCache.CacheStyle.Immutable);

            var baseUrl = message.BodyUrl;
            if (!baseUrl.StartsWith("http"))
            {
                baseUrl = null; // We use the default
            }

            message.Body = Execute(request, response =>
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
            }, baseUrl);
        }

        void AppendSystemMessages(IRestRequest request)
        {
            request.AddParameter("include_system_messages", settings.DisplaySystemMessages);
        }

        void AppendOrdering(IRestRequest request, string orderBy, bool ascending)
        {
            if (orderBy == null)
            {
                return;
            }

            request.AddParameter("sort", orderBy, ParameterType.GetOrPost);
            request.AddParameter("direction", ascending ? "asc" : "desc", ParameterType.GetOrPost);
        }

        void AppendPaging(IRestRequest request, int pageIndex)
        {
            request.AddParameter("page", pageIndex, ParameterType.GetOrPost);
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

        IRestClient CreateClient(string baseUrl = null)
        {
            var client = new RestClient(baseUrl ?? connection.Url)
            {
                Authenticator = new NtlmAuthenticator()
            };
            var deserializer = new JsonMessageDeserializer { DateFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK" };
            var xdeserializer = new XmlDeserializer();
            client.ClearHandlers();
            client.AddHandler("application/json", deserializer);
            client.AddHandler("text/json", deserializer);
            client.AddHandler("text/x-json", deserializer);
            client.AddHandler("text/javascript", deserializer);

            client.AddHandler("application/xml", xdeserializer);
            client.AddHandler("text/xml", xdeserializer);
            client.AddHandler("*", xdeserializer);

            client.AddDefaultHeader("Accept-Encoding", "gzip,deflate");

            return client;
        }

        static RestRequestWithCache CreateMessagesRequest(string endpointName = null) => endpointName != null
    ? new RestRequestWithCache(string.Format(EndpointMessagesEndpoint, endpointName), RestRequestWithCache.CacheStyle.IfNotModified)
    : new RestRequestWithCache(MessagesEndpoint, RestRequestWithCache.CacheStyle.IfNotModified);

        PagedResult<T> GetPagedResult<T>(RestRequestWithCache request) where T : class, new()
        {
            var result = Execute<PagedResult<T>, List<T>>(request, response => new PagedResult<T>
            {
                Result = response.Data,
                TotalCount = int.Parse(response.Headers.First(x => x.Name == ServiceControlHeaders.TotalCount).Value.ToString())
            });

            return result;
        }

        T GetModel<T>(RestRequestWithCache request)
where T : class, new() => Execute<T, T>(request, response => response.Data);

        T Execute<T>(RestRequestWithCache request, Func<IRestResponse, T> selector, string baseUrl = null)
        {
            var cacheStyle = request.CacheSyle;
            var restClient = CreateClient(baseUrl);

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

            var response = restClient.Execute(request);

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

        T Execute<T, T2>(RestRequestWithCache request, Func<IRestResponse<T2>, T> selector)
            where T : class, new()
            where T2 : class, new()
        {
            var cacheStyle = request.CacheSyle;
            var restClient = CreateClient();

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

            var response = restClient.Execute<T2>(request);

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

            eventAggregator.PublishOnUIThread(new AsyncOperationFailed(errorMessage));
            LogTo.Error(exception, errorMessage);
        }

        static bool HasSucceeded(IRestResponse response) => successCodes.Any(x => response != null && x == response.StatusCode && response.ErrorException == null);

        static IEnumerable<HttpStatusCode> successCodes = new[] { HttpStatusCode.OK, HttpStatusCode.Accepted, HttpStatusCode.NotModified, HttpStatusCode.NoContent };
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