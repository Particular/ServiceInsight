namespace ServiceInsight.ServiceControl
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ServiceInsight.ExtensionMethods;

    public class ServiceControlClientRegistry
    {
        readonly Func<string, IServiceControl> serviceControlFactory;
        readonly ConcurrentDictionary<string, IServiceControl> serviceControlClientCache;

        public ServiceControlClientRegistry(Func<string, IServiceControl> serviceControlFactory)
        {
            this.serviceControlFactory = serviceControlFactory;
            serviceControlClientCache = new ConcurrentDictionary<string, IServiceControl>(StringComparer.InvariantCultureIgnoreCase);
        }

        public void EnsureServiceControlClient(string serviceUrl)
        {
            if (!serviceUrl.IsValidUrl())
            {
                return;
            }

            var normalizeUrl = GetNormalizedUrl(serviceUrl);

            if (!serviceControlClientCache.ContainsKey(normalizeUrl))
            {
                var serviceControl = serviceControlFactory(normalizeUrl);
                serviceControlClientCache.TryAdd(normalizeUrl, serviceControl);
            }
        }

        public void RemoveServiceControlClient(string serviceUrl)
        {
            if (!serviceUrl.IsValidUrl())
            {
                return;
            }

            var normalizeUrl = GetNormalizedUrl(serviceUrl);

            if (serviceControlClientCache.ContainsKey(normalizeUrl))
            {
                serviceControlClientCache.TryRemove(normalizeUrl, out _);
            }
        }

        string GetNormalizedUrl(string serviceUrl)
        {
            if (serviceUrl.EndsWith("/"))
            {
                serviceUrl = serviceUrl.Remove(serviceUrl.Length - 1);
            }

            var builder = new UriBuilder(serviceUrl);

            return builder.Uri.ToString();
        }

        public virtual async Task<IEnumerable<string>> GetVersions()
        {
            var versions = new List<string>();
            foreach (var entry in serviceControlClientCache)
            {
                var version = await entry.Value.GetVersion();
                versions.Add(version);
            }
            return versions;
        }

        public virtual IServiceControl GetServiceControl(string url)
        {
            var normalizeUrl = GetNormalizedUrl(url);
            return serviceControlClientCache[normalizeUrl];
        }

        public bool IsRegistered(string url)
        {
            var normalizedUrl = GetNormalizedUrl(url);
            return serviceControlClientCache.ContainsKey(normalizedUrl);
        }
    }
}