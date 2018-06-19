namespace ServiceInsight.ExtensionMethods
{
    using RestSharp;
    using RestSharp.Deserializers;

    public static class RestClientExtensions
    {
        public static void AddJsonDeserializer(this IRestClient client, IDeserializer deserializer)
        {
            client.AddHandler("application/json", deserializer);
            client.AddHandler("text/json", deserializer);
            client.AddHandler("text/x-json", deserializer);
            client.AddHandler("text/javascript", deserializer);
        }

        public static void AddXmlDeserializer(this IRestClient client, IDeserializer deserializer)
        {
            client.AddHandler("application/xml", deserializer);
            client.AddHandler("text/xml", deserializer);
            client.AddHandler("*", deserializer);
        }
    }
}