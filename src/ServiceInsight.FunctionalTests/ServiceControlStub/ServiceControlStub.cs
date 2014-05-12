namespace Particular.ServiceInsight.FunctionalTests.ServiceControlStub
{
    using System;
    using System.Web.Http;
    using System.Web.Http.SelfHost;
    using Castle.Core.Logging;
    using TestStack.White.Configuration;

    public class ServiceControl : IDisposable
    {
        readonly ILogger logger = CoreAppXmlConfiguration.Instance.LoggerFactory.Create(typeof(ServiceControl));
        readonly HttpSelfHostServer server;

        const int StubServicePort = 55555;
        const string StubServiceUrl = "http://localhost";

        ServiceControl()
        {
            var url = GetBaseUrl();
            var config = new HttpSelfHostConfiguration(url);

            Configure(config);

            server = new HttpSelfHostServer(config);
            server.OpenAsync().Wait();

            logger.DebugFormat("ServiceControl stub started at {0}", url);
        }

        public static string GetUrl()
        {
            return GetBaseUrl() + "api";
        }

        static string GetBaseUrl()
        {
            return string.Format(StubServiceUrl + ":" + StubServicePort + "/");
        }

        static void Configure(HttpSelfHostConfiguration config)
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
            server.CloseAsync().Wait();
            server.Dispose();
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