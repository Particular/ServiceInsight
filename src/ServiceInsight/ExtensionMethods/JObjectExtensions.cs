namespace ServiceInsight.ExtensionMethods
{
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class JObjectExtensions
    {
        public static string GetFormatted(this JObject document)
        {
            if (document == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            using (var writer = new JsonTextWriter(new StringWriter(sb)))
            {
                writer.Formatting = Formatting.Indented;
                document.WriteTo(writer);
            }

            return sb.ToString();
        }
    }
}