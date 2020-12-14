using System;
using System.Collections.Concurrent;
using ServiceInsight.ExtensionMethods;

namespace ServiceInsight.ServiceControl
{
    public class ServiceControlClientRegistry
    {
        readonly Func<string, IServiceControl> serviceControlFactory;
        readonly ConcurrentDictionary<string, IServiceControl> serviceControlClientCache;

        public ServiceControlClientRegistry(Func<string, IServiceControl> serviceControlFactory)
        {
            this.serviceControlFactory = serviceControlFactory;
            this.serviceControlClientCache = new ConcurrentDictionary<string, IServiceControl>();
        }
        
        public void EnsureServiceControlClient(string serviceUrl)
        {
            if (!serviceUrl.IsValidUrl())
            {
                return;
            }

            if (!serviceControlClientCache.ContainsKey(serviceUrl))
            {
                var serviceControl = serviceControlFactory(serviceUrl);
                serviceControlClientCache.TryAdd(serviceUrl, serviceControl);
            }
        }

        public IServiceControl GetServiceControl(string url)
        {
            return serviceControlClientCache[url];
        }
    }
}