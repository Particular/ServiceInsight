namespace ServiceInsight.Framework.Events
{
    using ServiceInsight.Models;

    public class RefreshEndpointMessages
    {
        public RefreshEndpointMessages(Endpoint endpoint, int pageIndex = 1, string searchQuery = null, string orderBy = null, bool ascending = false)
        {
            Endpoint = endpoint;
            PageIndex = pageIndex;
            SearchQuery = searchQuery;
            OrderBy = orderBy;
            Ascending = ascending;
        }

        public Endpoint Endpoint { get; }

        public int PageIndex { get; }

        public string SearchQuery { get; }

        public string OrderBy { get; }

        public bool Ascending { get; }
    }
}