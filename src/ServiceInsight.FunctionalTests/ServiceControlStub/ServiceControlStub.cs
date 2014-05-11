namespace Particular.ServiceInsight.FunctionalTests.ServiceControlStub
{
    using System;
    using System.Diagnostics;
    using System.Security.Principal;
    using System.Web.Http;
    using System.Web.Http.SelfHost;
    using Castle.Core.Logging;
    using TestStack.White.Configuration;

    public class ServiceControl : IDisposable
    {
        private readonly ILogger _logger = CoreAppXmlConfiguration.Instance.LoggerFactory.Create(typeof(ServiceControl));
        private readonly HttpSelfHostServer _server;

        public const int StubServicePort = 55555;
        public const string StubServiceUrl = "http://localhost";

        private ServiceControl()
        {
            var url = GetBaseUrl();
            var config = new HttpSelfHostConfiguration(url);

            Configure(config);

            _server = new HttpSelfHostServer(config);
            _server.OpenAsync().Wait();

            _logger.DebugFormat("ServiceControl stub started at {0}", url);
        }

        public static string GetUrl()
        {
            return GetBaseUrl() + "api";
        }

        private static string GetBaseUrl()
        {
            return string.Format(StubServiceUrl + ":" + StubServicePort + "/");
        }

        private static void Configure(HttpSelfHostConfiguration config)
        {
            config.Routes.MapHttpRoute("Default Route", "api/{controller}", new { controller = "Home" });
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            config.Filters.Add(new ServiceControlErrorFilter());
            config.Filters.Add(new VersionInformationFilter());
        }

        public static ServiceControl Start()
        {
            return new ServiceControl();
        }

        public void Stop()
        {
            _server.CloseAsync().Wait();
            _server.Dispose();
        }

        void IDisposable.Dispose()
        {
            Stop();
        }

        public static void RunAsConsole()
        {
            Start();
            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}