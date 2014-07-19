namespace Particular.ServiceInsight.FunctionalTests.Services.TestDataBuilders
{
    using System.Collections.Generic;
    using Desktop.Models;

    public class EndpointBuilder
    {
        private List<Endpoint> endpoints;

        public EndpointBuilder()
        {
            endpoints = new List<Endpoint>
            {
                new Endpoint
                {
                    Name = "Sales",
                    HostDisplayName = "localhost"
                },
                new Endpoint
                {
                    Name = "CustomerRelations",
                    HostDisplayName = "localhost"
                }
            };
        }

        public EndpointBuilder WithEndpoints(params Endpoint[] endpoints)
        {
            this.endpoints = new List<Endpoint>();
            this.endpoints.AddRange(endpoints);

            return this;
        }

        public void Build()
        {
            TestDataWriter.Write("GetEndpoints", endpoints);
        }
    }
}