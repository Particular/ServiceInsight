namespace ServiceInsight.ServiceControl
{
    using RestSharp;

    public class RestRequestWithCache : RestRequest
    {
        public RestRequestWithCache(CacheStyle cacheStyle)
            : this(null, cacheStyle)
        {
        }

        public RestRequestWithCache(string resource, CacheStyle cacheStyle)
            : this(resource, Method.GET, cacheStyle)
        {
        }

        public RestRequestWithCache(string resource, Method method, CacheStyle cacheStyle = CacheStyle.None)
            : base(resource, method)
        {
            CacheSyle = cacheStyle;
            DateFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
        }

        public CacheStyle CacheSyle { get; set; }

        public enum CacheStyle
        {
            None,
            Immutable,
            IfNotModified
        }
    }
}