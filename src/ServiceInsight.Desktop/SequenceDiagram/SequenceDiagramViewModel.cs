using System.Collections.Generic;

namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    public class SequenceDiagramViewModel
    {
        public SequenceDiagramViewModel()
        {
            Endpoints = new[]
            {
                new EndpointInfo { Name = "ECommerce", Version = "4.6.1", Host = "@machinename", Active = "Yes" },
                new EndpointInfo { Name = "Sales", Version = "4.6.1", Host = "@machinename", Active = "Yes" },
                new EndpointInfo { Name = "CustomerRelations", Version = "4.6.1", Host = "@machinename", Active = "Yes" },
                new EndpointInfo { Name = "ContentManagement", Version = "4.6.1", Host = "@machinename", Active = "Yes" },
                new EndpointInfo { Name = "Operations", Version = "4.6.1", Host = "@machinename", Active = "Yes" },
            };

            Messages = new[]
            {
                new MessageInfo { Endpoints = Endpoints, Name = "SubmitOrder", IsStartMessage = true, FromEndpoint = "ECommerce", ToEndpoints = new[] { "Sales" } },
                new MessageInfo { Endpoints = Endpoints, Name = "OrderPlaced", FromEndpoint = "Sales", ToEndpoints = new[] { "ECommerce" }, TimeoutEndpoints = new[] { "Sales" } },
                new MessageInfo { Endpoints = Endpoints, Name = "OrderAccepted", FromEndpoint = "Sales", ToEndpoints = new[] { "CustomerRelations", "ContentManagement" }, },
                new MessageInfo { Endpoints = Endpoints, Name = "ProvisionDownloadRequest", FromEndpoint = "ContentManagement", ToEndpoints = new[] { "Operations" }, },
                new MessageInfo { Endpoints = Endpoints, Name = "ProvisionDownloadResponse", FromEndpoint = "Operations", ToEndpoints = new[] { "ContentManagement" }, },
                new MessageInfo { Endpoints = Endpoints, Name = "DownloadIsReady", FromEndpoint = "ContentManagement", ToEndpoints = new[] { "ECommerce" }, },
            };
        }

        public IEnumerable<EndpointInfo> Endpoints { get; set; }

        public IEnumerable<MessageInfo> Messages { get; set; }
    }
}