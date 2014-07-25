namespace Particular.ServiceInsight.FunctionalTests.DataBuilders
{
    using System.Collections.Generic;
    using System.Linq;
    using Desktop.Models;
    using Services;

    public class EndpointListBuilder
    {
        private List<Endpoint> defaultEndpoints;

        public EndpointListBuilder()
        {
            defaultEndpoints = new List<Endpoint>();
        }

        public EndpointListBuilder WithEndpoints(params string[] endpoints)
        {
            defaultEndpoints = new List<Endpoint>(endpoints.Select(e => new Endpoint { Name = e }));
            return this;
        }

        public void Build()
        {
            TestDataWriter.Write("GetEndpoints", defaultEndpoints);
        }
    }
}