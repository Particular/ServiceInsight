using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Caliburn.Core.Logging;
using NServiceBus.Profiler.Common.Models;
using RestSharp;
using System.Linq;

namespace NServiceBus.Profiler.Common.ExtensionMethods
{
    public static class RestSharpExtensions
    {
        private static readonly ILog Logger = LogManager.GetLog(typeof (IRestClient));

        public static Task<PagedResult<T>> GetPagedResult<T>(this IRestClient client, IRestRequest request)
            where T : class, new()
        {
            var completionSource = new TaskCompletionSource<PagedResult<T>>();
            client.ExecuteAsync<List<T>>(request, response =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    completionSource.SetResult(new PagedResult<T>
                    {
                        Result = response.Data,
                        TotalCount = int.Parse(response.Headers.First(x => x.Name == "Total-Count").Value.ToString())
                    });
                }
                else
                {
                    var errorMessage = string.Format("Unknown error connecting to the service at {0}, Http Status code is {1}", client.BuildUri(request), response.StatusCode);
                    Logger.Error(errorMessage, response.ErrorException);
                    completionSource.SetResult(new PagedResult<T>
                    {
                        Result = new List<T>(),
                        TotalCount = 0
                    });
                }
            });
            return completionSource.Task;
        }

        public static Task<T> GetModelAsync<T>(this IRestClient client, IRestRequest request) 
            where T : class, new()
        {
            return ExecuteAsync<T>(client, request, response => response.Data);
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