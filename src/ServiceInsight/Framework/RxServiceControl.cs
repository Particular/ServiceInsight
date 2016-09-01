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
    using ReactiveUI;
    using Serilog;

    interface IRxServiceControl
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

        public static IRxServiceControl Instance { get; set; }

        readonly IBlobCache cache;

        List<string> serviceControlUrls;

        Subject<Unit> manualRefresh;
        Subject<IObservable<Unit>> trigger;
        IObservable<Unit> subject;

        public RxServiceControl(IBlobCache cache)
        {
            this.cache = cache;

            serviceControlUrls = new List<string>();

            manualRefresh = new Subject<Unit>();
            trigger = new Subject<IObservable<Unit>>();

            subject = Observable.Merge(
                manualRefresh,
                trigger.Switch());
        }

        public Task Refresh()
        {
            manualRefresh.OnNext(Unit.Default);
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

        public IObservable<ServiceControlData> Endpoints()
        {
            return subject.SelectMany(_ =>
            {
                var data = serviceControlUrls.Select(url =>
                    GetData($"{url}/endpoints")
                    .Select(d => new ServiceControlData(url, d)));

                return Observable.Merge(data);
            }).Publish().RefCount();
        }

        public IObservable<ServiceControlData> Messages()
        {
            return subject.SelectMany(_ =>
            {
                var data = serviceControlUrls.Select(url =>
                    GetData($"{url}/messages")
                    .Select(d => new ServiceControlData(url, d)));

                return Observable.Merge(data);
            }).Publish().RefCount();
        }

        public IObservable<ServiceControlData> EndpointMessages(string endpointName)
        {
            return subject.SelectMany(_ =>
            {
                var data = serviceControlUrls.Select(url =>
                    GetData($"{url}/endpoints/{endpointName}/messages/")
                    .Select(d => new ServiceControlData(url, d)));

                return Observable.Merge(data);
            }).Publish().RefCount();
        }

        private IObservable<IEnumerable<JObject>> GetData(string url)
        {
            LogTo.Information("Rx HTTP {url}", url);

            return cache.GetOrFetchWithETag(url)
                .Select(j => JArray.Parse(j).Cast<JObject>());
        }

        public void SetRefresh(TimeSpan interval)
        {
            trigger.OnNext(Observable.Interval(interval).Select(_ => Unit.Default).ObserveOn(RxApp.MainThreadScheduler));
        }

        public void DisableRefresh()
        {
            trigger.OnNext(Observable.Never<Unit>());
        }
    }

    class ServiceControlData
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