using System;
using System.Net;
using System.Threading.Tasks;
using Caliburn.Core.Logging;
using RestSharp;

namespace NServiceBus.Profiler.Common.ExtensionMethods
{
    public static class RestSharpExtensions
    {
        private static readonly ILog Logger = LogManager.GetLog(typeof (IRestClient));

        public static Task<T> GetModelAsync<T>(this IRestClient client, IRestRequest request) 
            where T : class, new()
        {
            return ExecuteAsync<T>(client, request, response =>
            {
                return response.Data;
            });
        }

        private static Task<T> ExecuteAsync<T>(this IRestClient client, IRestRequest request, Func<IRestResponse<T>, T> selector)
            where T : class, new()
        {
            var completionSource = new TaskCompletionSource<T>();
            client.ExecuteAsync<T>(request, response =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    completionSource.SetResult(selector(response));
                }
                else
                {
                    var errorMessage = string.Format("Unknown error connecting to the service at {0}, Http Status code is {1}", client.BuildUri(request), response.StatusCode);
                    Logger.Error(errorMessage, response.ErrorException);
                    completionSource.SetResult(null);
                }
            });
            return completionSource.Task;
        }

    }
}