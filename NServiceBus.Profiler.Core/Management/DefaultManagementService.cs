using System.Net;
using RestSharp;

namespace NServiceBus.Profiler.Core.Management
{
    public class DefaultManagementService : IManagementService
    {
        public object GetAuditMessages()
        {
            return "Management message";
        }

        public bool IsAlive(string connectedToService)
        {
            var client = new RestSharp.RestClient(connectedToService);
            var request = new RestRequest("ping");
            var response = client.Execute<VersionInfo>(request);

            return response.StatusCode == HttpStatusCode.OK && response.Data != null;
        }
    }
}