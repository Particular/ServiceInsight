using System;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Castle.Core.Logging;
using TestStack.White.Configuration;

namespace NServiceBus.Profiler.FunctionalTests.ServiceControlStub
{
    public class ServiceControl : IDisposable
    {
        private readonly ILogger _logger = CoreAppXmlConfiguration.Instance.LoggerFactory.Create(typeof(ServiceControl));
        private readonly HttpSelfHostServer _server;

        public const string StubServiceUrl = "http://localhost:55555";

        private ServiceControl()
        {
            var config = new HttpSelfHostConfiguration(StubServiceUrl);

            Configure(config);

            _server = new HttpSelfHostServer(config);
            _server.OpenAsync().Wait();

            _logger.DebugFormat("ServiceControl stub started at {0}", StubServiceUrl);
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