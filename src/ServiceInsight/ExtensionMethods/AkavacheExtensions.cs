namespace ServiceInsight.ExtensionMethods
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Reactive.Linq;
    using Akavache;
    using Anotar.Serilog;
    using Framework;
    using Serilog;

    public static class AkavacheExtensions
    {
        static ILogger anotarLogger = Log.ForContext<IRxServiceControl>();

        public static IObservable<string> GetOrFetchWithETag(this IBlobCache cache, string url)
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

            return result
                .Concat(fetch)
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
}