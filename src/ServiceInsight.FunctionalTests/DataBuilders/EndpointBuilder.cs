namespace Particular.ServiceInsight.FunctionalTests.DataBuilders
{
    using System.Collections.Generic;
    using Desktop.Models;
    using Services;

    public class EndpointBuilder
    {
        private List<Endpoint> defaultEndpoints;

        public EndpointBuilder()
        {
            defaultEndpoints = new List<Endpoint>
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
            defaultEndpoints = new List<Endpoint>();
            defaultEndpoints.AddRange(endpoints);

            return this;
        }

        public void Build()
        {
            TestDataWriter.Write("GetEndpoints", defaultEndpoints);
        }
    }
}