namespace ServiceInsight.ServiceControl
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using IdentityModel.OidcClient.Browser;

    public class SystemBrowser : IBrowser
    {
        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            var result = await RunListener(options, cancellationToken);

            return result;
        }

        public static async Task<string> GetLocalRedirectUrl()
        {
            var selectedPort = await SelectPortAsync();
            var redirectUrl = $"http://localhost:{selectedPort}/ServiceInsight/";
            return redirectUrl;
        }

        public static void OpenBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true,
                });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
        }

        /// <summary>
        /// This method is designed to run on a background thread, running a most simple Httplistener
        /// waiting for the OAUTH code to come back. This method supports cancellation through the provided
        /// <paramref name="cancellationToken"/> and will throw OperationCancelledException if the operation is cancelled.
        /// </summary>
        async Task<BrowserResult> RunListener(BrowserOptions options, CancellationToken cancellationToken)
        {
            var loginurl = options.StartUrl;
            var redirectUrl = options.EndUrl;

            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add(redirectUrl);
                try
                {
                    listener.Start();
                    Debug.WriteLine($"Starting listening for OAUTH code in url {redirectUrl}");

                    // Don't open browser until listener is operating because if we're already logged in response could be immediate
                    OpenBrowser(loginurl);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        // Wait for the context and possibly the cancellation of the operation.
                        var contextTask = await GetCancellableTaskAsync(
                            listener.GetContextAsync(),
                            cancellationToken);

                        // Awaits the resulting task. If the task is the cancelled one it will throw, otherwise
                        // it will just return the context.
                        var context = await contextTask;

                        // Ensure that this is the URL that we expect, Internet Explorer, and possibly other browsers,
                        // might ask for the favicon or other metadata pages before actually redirecting with the code.
                        if (context.Request.Url.AbsolutePath != "/ServiceInsight/")
                        {
                            Debug.WriteLine($"Got request for path {context.Request.Url}");
                            using (var response = context.Response)
                            {
                                response.StatusCode = 404;
                            }
                            continue;
                        }

                        using (var response = context.Response)
                        {
                            response.ContentType = "text/plain";
                            response.StatusCode = 200;
                            using (var writer = new StreamWriter(response.OutputStream, Encoding.UTF8))
                            {
                                writer.Write("Authentication successful. You can close this browser window.");
                            }
                        }

                        // Wait for a bit for the reponse to be sent.
                        await Task.Delay(500, CancellationToken.None);

                        return new BrowserResult
                        {
                            ResultType = BrowserResultType.Success,
                            Response = context.Request.Url.Query
                        };

                        var accessCode = context.Request.QueryString["code"];
                        var error = context.Request.QueryString["error"];

                        // Redirect to the appropiate website depending on success or failure of the login
                        // operation.
                        using (var response = context.Response)
                        {
                            Debug.WriteLineIf(string.IsNullOrEmpty(accessCode), $"Failed to authenticate the user OAUTH login flow.");
                            response.StatusCode = 303;
                            //response.RedirectLocation = string.IsNullOrEmpty(accessCode) ? _failureUrl : _successUrl;
                        }

                        // Wait for a bit for the reponse to be sent.
                        await Task.Delay(500, CancellationToken.None);

                        //return new FlowResult { AccessCode = accessCode, Error = error };

                        return new BrowserResult
                        {
                            ResultType = !string.IsNullOrEmpty(accessCode) ? BrowserResultType.Success : BrowserResultType.UnknownError,
                            Response = accessCode
                        };
                    }
                }
                finally
                {
                    Debug.WriteLine($"Shutting down listener for redirect url {redirectUrl}");
                    listener.Stop();
                }
            }

            return new BrowserResult { ResultType = BrowserResultType.UserCancel };
        }

        /// <summary>
        /// This method selects a port on which to run.
        /// </summary>
        static Task<int> SelectPortAsync()
        {
            return Task.Run(() =>
            {
                var listener = new TcpListener(IPAddress.Loopback, 0);
                try
                {
                    listener.Start();
                    return ((IPEndPoint)listener.LocalEndpoint).Port;
                }
                finally
                {
                    listener.Stop();
                }
            });
        }

        /// <summary>
        /// Returns a task that can be awaited to get the task with the data. (There are two levels of await here). If
        /// the operation completed normally then <paramref name="sourceTask"/> is returend and when awaited the result of the
        /// operation is returned. If the operation is cancelled then a cancelled dummy task is returned, when awaited
        /// <seealso cref="OperationCanceledException"/> is thrown. This way any task can be made cancellable if the original
        /// source of the task doesn't support cancellation.
        /// </summary>
        static Task<Task<T>> GetCancellableTaskAsync<T>(Task<T> sourceTask, CancellationToken cancellationToken)
        {
            var taskSource = new TaskCompletionSource<T>();
            cancellationToken.Register(() => taskSource.TrySetCanceled());
            return Task.WhenAny(sourceTask, taskSource.Task);
        }
    }
}