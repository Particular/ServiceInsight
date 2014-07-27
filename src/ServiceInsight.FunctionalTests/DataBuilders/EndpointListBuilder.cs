namespace Particular.ServiceInsight.FunctionalTests.DataBuilders
{
    using System.Collections.Generic;
    using System.Linq;
    using Desktop.Models;
    using Services;

    public class EndpointListBuilder
    {
        private List<Endpoint> endpoints;

        public EndpointListBuilder()
        {
            endpoints = new List<Endpoint>();
        }

        public EndpointListBuilder WithEndpoints(params string[] endpointList)
        {
            endpoints.AddRange(endpointList.Select(e => new Endpoint { Name = e }));
            return this;
        }

        public EndpointListBuilder WithEndpoints(params Endpoint[] endpointList)
        {
            endpoints.AddRange(endpointList);
            return this;
        }

        public void Build()
        {
            TestDataWriter.Write("GetEndpoints", endpoints);
        }
    }
}