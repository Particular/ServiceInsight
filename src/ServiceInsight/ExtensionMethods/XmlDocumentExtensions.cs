namespace ServiceInsight.ExtensionMethods
{
    using System.IO;
    using System.Text;
    using System.Xml;

    public static class XmlDocumentExtensions
    {
        public static string GetFormatted(this XmlDocument document)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            using (var xtw = XmlWriter.Create(sw, new XmlWriterSettings
            {
                Indent = true
            }))
            {
                document.WriteTo(xtw);
            }

            return sb.ToString();
        }
    }
}