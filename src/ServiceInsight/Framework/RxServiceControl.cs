namespace ServiceInsight.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using Akavache;
    using Anotar.Serilog;
    using ExtensionMethods;
    using Newtonsoft.Json.Linq;
    using Pirac;
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
            timerTriggerProvider.OnNext(Observable.Interval(interval).Select(_ => Unit.Default).ObserveOnPiracMain());
        }

        public void DisableRefresh()
        {
            timerTriggerProvider.OnNext(Observable.Never<Unit>());
        }

        private IObservable<IEnumerable<JObject>> GetData(string url)
        {
            LogTo.Information("Rx HTTP {url}", url);

            return cache.GetOrFetchWithETag(url)
                .Select(j => JArray.Parse(j).Cast<JObject>());
        }

        private IObservable<ServiceControlData> InitializeStream(string url)
        {
            return trigger.SelectMany(_ =>
            {
                var data = serviceControlUrls.Select(baseurl =>
                    GetData($"{baseurl}/{url}")
                    .Select(d => new ServiceControlData(baseurl, d)));

                return Observable.Merge(data);
            }).Publish().RefCount();
        }
    }

    public class ServiceControlData
    {
        public ServiceControlData(string url, IEnumerable<JObject> data)
        {
            Data = data;
            Url = url;
        }

        public string Url { get; }

        public IEnumerable<dynamic> Data { get; }
    }
}