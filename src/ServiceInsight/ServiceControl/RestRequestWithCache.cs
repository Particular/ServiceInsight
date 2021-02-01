namespace ServiceInsight.ServiceControl
{
    using RestSharp;

    public class RestRequestWithCache : RestRequest
    {
        public const string CacheDateFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

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