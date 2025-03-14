namespace ServiceInsight.ServiceControl
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using ServiceInsight.ExtensionMethods;

    public class ServiceControlClientRegistry
    {
        readonly Func<string, string, SecureString, IServiceControl> serviceControlFactory;
        readonly ConcurrentDictionary<string, IServiceControl> serviceControlClientCache;
        readonly ConcurrentDictionary<string, Tuple<string, SecureString>> credentialsCache;

        public ServiceControlClientRegistry(Func<string, string, SecureString, IServiceControl> serviceControlFactory)
        {
            this.serviceControlFactory = serviceControlFactory;
            serviceControlClientCache = new ConcurrentDictionary<string, IServiceControl>(StringComparer.InvariantCultureIgnoreCase);
            credentialsCache = new ConcurrentDictionary<string, Tuple<string, SecureString>>(StringComparer.InvariantCultureIgnoreCase);
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
                var credentials = credentialsCache.TryGetValue(normalizeUrl, out var creds) ? creds : null;
                var username = credentials?.Item1;
                var password = credentials?.Item2;
                var serviceControl = Create(normalizeUrl, username, password);
                serviceControlClientCache.TryAdd(normalizeUrl, serviceControl);
            }
        }

        public IServiceControl Create(string url, string username = null, SecureString password = null)
        {
            var serviceControl = serviceControlFactory(url, username, password);
            // Store credentials for future use
            var normalizedUrl = GetNormalizedUrl(url);
            if (!string.IsNullOrEmpty(username) || password.Length > 0)
            {
                credentialsCache[normalizedUrl] = Tuple.Create(username, password);
            }
            return serviceControl;
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
                var (version, _) = await entry.Value.GetVersion();
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