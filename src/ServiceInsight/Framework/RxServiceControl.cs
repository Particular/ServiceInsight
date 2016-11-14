namespace ServiceInsight.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using Akavache;
    using Anotar.Serilog;
    using Newtonsoft.Json.Linq;
    using Serilog;

    public interface IRxServiceControl
    {
        Task Refresh();

        Task<bool> AddServiceControl(string url);

        Task<bool> RemoveServiceControl(string url);

        Task ClearServiceControls();

        IObservable<ServiceControlData> Endpoints();

        IObservable<ServiceControlData> Messages();

        IObservable<ServiceControlData> EndpointMessages(string endpointName);

        void SetRefresh(TimeSpan interval);

        void DisableRefresh();
    }

    class RxServiceControl : IRxServiceControl
    {
        static Serilog.ILogger anotarLogger = Log.ForContext<IRxServiceControl>();

        readonly IBlobCache cache;

        List<string> serviceControlUrls;

        IObserver<Unit> manualRefreshTrigger;
        IObserver<IObservable<Unit>> timerTriggerProvider;
        IObservable<Unit> trigger;

        IObservable<ServiceControlData> endpointsStream;
        IObservable<ServiceControlData> messagesStream;
        Dictionary<string, IObservable<ServiceControlData>> endpointMessagesStreams;

        public RxServiceControl(IBlobCache cache)
        {
            this.cache = cache;

            serviceControlUrls = new List<string>();

            var manualRefresh = new Subject<Unit>();
            manualRefreshTrigger = manualRefresh.AsObserver();
            var timerProvider = new Subject<IObservable<Unit>>();
            timerTriggerProvider = timerProvider.AsObserver();

            trigger = Observable.Merge(
                manualRefresh,
                timerProvider.Switch());

            endpointsStream = InitializeStream("endpoints");
            messagesStream = InitializeStream("messages");
            endpointMessagesStreams = new Dictionary<string, IObservable<ServiceControlData>>(StringComparer.OrdinalIgnoreCase);
        }

        public Task Refresh()
        {
            manualRefreshTrigger.OnNext(Unit.Default);
            return Task.FromResult(0);
        }

        public async Task<bool> AddServiceControl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException($"{nameof(url)} is null or empty.", nameof(url));
            }

            url = url.TrimEnd('/');
            using (var client = new HttpClient())
            {
                try
                {
                    var page = (await client.GetStringAsync(url)).Trim();
                    JObject.Parse(page);
                    serviceControlUrls.Add(url);
                    return true;
                }
                catch
                {
                    // TODO need to log invalid url
                    return false;
                }
            }
        }

        public Task<bool> RemoveServiceControl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException($"{nameof(url)} is null or empty.", nameof(url));
            }

            return Task.FromResult(serviceControlUrls.Remove(url.TrimEnd('/')));
        }

        public Task ClearServiceControls()
        {
            serviceControlUrls.Clear();
            return Task.FromResult(0);
        }

        public IObservable<ServiceControlData> Endpoints() => endpointsStream;

        public IObservable<ServiceControlData> Messages() => messagesStream;

        public IObservable<ServiceControlData> EndpointMessages(string endpointName)
        {
            var url = $"endpoints/{endpointName}/messages/";
            IObservable<ServiceControlData> result;
            if (!endpointMessagesStreams.TryGetValue(url, out result))
            {
                result = InitializeStream(url);
                endpointMessagesStreams.Add(url, result);
            }
            return result;
        }

        public void SetRefresh(TimeSpan interval)
        {
            timerTriggerProvider.OnNext(Observable.Interval(interval).Select(_ => Unit.Default));
        }

        public void DisableRefresh()
        {
            timerTriggerProvider.OnNext(Observable.Never<Unit>());
        }

        private IObservable<ServiceControlData> InitializeStream(string url)
        {
            return trigger.SelectMany(_ =>
            {
                var data = serviceControlUrls.Select(baseurl =>
                    GetData($"{baseurl}/{url}")
                    .Select(d => new ServiceControlData(baseurl, d.Item1, d.Item2)));

                return Observable.Merge(data);
            }).Publish().RefCount();
        }

        private IObservable<Tuple<bool, IEnumerable<JObject>>> GetData(string url)
        {
            return GetOrFetchWithETag(cache, url)
                .Select(j => Tuple.Create(j.Item1, JArray.Parse(j.Item2).Cast<JObject>()));
        }

        private static IObservable<Tuple<bool, string>> GetOrFetchWithETag(IBlobCache cache, string url)
        {
            var result =
                // Get from cache
                cache.GetObject<string>(url)

                // Cached values are true
                .Select(x => Tuple.Create(x, true))

                // Turn exceptions into false
                .Catch(Observable.Return(Tuple.Create(string.Empty, false)))

                // If true, return an observable with the result, else an empty observable.
                .SelectMany(x => x.Item2 ? Observable.Return(x.Item1) : Observable.Empty<string>());

            var fetch =
                // Get the ETag from cache
                cache.GetObject<string>("etag-" + url)

                // Exceptions => Blank ETag
                .Catch(Observable.Return(string.Empty))

                // Call our web method
                .SelectMany(etag => GetFromWeb(url, etag)

                    // Invalidate the old and add the new etag to the cache
                    .SelectMany(x => cache.InvalidateObject<string>("etag-" + url).Select(_ => x))
                    .SelectMany(x => cache.InsertObject("etag-" + url, x.Item1).Select(_ => x))

                    // Invalidate the old and add the new data to the cache
                    .SelectMany(x => cache.InvalidateObject<string>(url).Select(_ => x))
                    .SelectMany(x => cache.InsertObject(url, x.Item2).Select(_ => x)))

                // Select the data from the tuple
                .Select(x => x.Item2);

            return result.Select(s => Tuple.Create(true, s))
                .Concat(fetch.Select(s => Tuple.Create(false, s)))
                .Replay()
                .RefCount();
        }

        private static HttpClient CreateWebClient()
        {
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            var client = new HttpClient(handler);
            return client;
        }

        private static IObservable<Tuple<string, string>> GetFromWeb(string url, string etag)
        {
            return Observable.Create<Tuple<string, string>>(async observer =>
            {
                using (var client = CreateWebClient())
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri(url)
                    };

                    if (!string.IsNullOrEmpty(etag))
                    {
                        request.Headers.TryAddWithoutValidation("If-None-Match", etag);
                    }

                    LogRequest(request);

                    var response = await client.SendAsync(request)
                        .ConfigureAwait(false);

                    LogResponse(response);

                    if (!response.IsSuccessStatusCode &&
                        response.StatusCode != HttpStatusCode.NotModified)
                    {
                        observer.OnError(new HttpRequestException(
                            "Status code: " + response.StatusCode));
                    }
                    else if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync()
                            .ConfigureAwait(false);

                        //observer.OnNext(Tuple.Create(response.Headers.ETag.Tag, data));

                        // Because SC doesn't send etags properly
                        var resultEtag = response.Headers.First(h => h.Key == "ETag").Value.First();
                        observer.OnNext(Tuple.Create(resultEtag, data));
                    }
                }
                observer.OnCompleted();
            });
        }

        private static void LogRequest(HttpRequestMessage request)
        {
            LogTo.Information("HTTP {Method:l} {url:l}", request.Method, request.RequestUri);

            foreach (var parameter in request.Properties)
            {
                LogTo.Debug("Request Parameter: {Name} : {Value}",
                                                       parameter.Key,
                                                       parameter.Value);
            }
        }

        private static void LogResponse(HttpResponseMessage response)
        {
            LogTo.Debug("HTTP Status {code} ({uri})", response.StatusCode, response.RequestMessage.RequestUri);

            foreach (var header in response.Headers)
            {
                LogTo.Debug("Response Header: {Name} : {Value}",
                                                     header.Key,
                                                     header.Value);
            }
        }
    }

    public class ServiceControlData
    {
        public ServiceControlData(string url, bool cached, IEnumerable<JObject> data)
        {
            Data = data;
            Url = url;
            Cached = cached;
        }

        public string Url { get; }

        public bool Cached { get; set; }

        public IEnumerable<dynamic> Data { get; }
    }
}