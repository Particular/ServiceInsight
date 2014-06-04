namespace Particular.ServiceInsight.Desktop.Core.MessageDecoders
{
    using System.Globalization;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;
    using RestSharp;
    using RestSharp.Deserializers;

    public class JsonMessageDeserializer : IDeserializer
    {
        class UnderscoreMappingResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                return Regex.Replace(
                    propertyName, @"([A-Z])([A-Z][a-z])|([a-z0-9])([A-Z])", "$1$3_$2$4").ToLower();
            }
        }

        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }

        public T Deserialize<T>(IRestResponse response)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new UnderscoreMappingResolver(),
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                Converters =
                {
                    new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.RoundtripKind},
                    new StringEnumConverter{CamelCaseText = true},
                }
            };

            var result = JsonConvert.DeserializeObject<T>(response.Content, serializerSettings);
            return result;
        }
    }
}