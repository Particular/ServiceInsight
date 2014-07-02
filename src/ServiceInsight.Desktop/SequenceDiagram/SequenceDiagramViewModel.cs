using System.Collections.Generic;

namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    public class SequenceDiagramViewModel
    {
        public SequenceDiagramViewModel()
        {
            Endpoints = new List<EndpointInfo>
            {
                new EndpointInfo { Name = "ECommerce", Version = "4.6.1", Host = "@machinename", Active = "Yes" },
                new EndpointInfo { Name = "Sales", Version = "4.6.1", Host = "@machinename", Active = "Yes" },
                new EndpointInfo { Name = "CustomerRelations", Version = "4.6.1", Host = "@machinename", Active = "Yes" },
                new EndpointInfo { Name = "ContentManagement", Version = "4.6.1", Host = "@machinename", Active = "Yes" },
                new EndpointInfo { Name = "Operations", Version = "4.6.1", Host = "@machinename", Active = "Yes" },
            };
        }

        public List<EndpointInfo> Endpoints { get; set; }
    }

    public class EndpointInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Host { get; set; }
        public string Active { get; set; }
    }
}