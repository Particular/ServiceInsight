using System.IO;
using System.Text;
using System.Xml;

namespace NServiceBus.Profiler.Common.ExtensionMethods
{
    public static class XmlDocumentExtensions
    {
         public static string GetFormatted(this XmlDocument document)
         {
             if (document.InnerXml.IsEmpty()) return string.Empty;

             var xd = new XmlDocument();
             xd.LoadXml(document.InnerXml);

             var sb = new StringBuilder();
             var sw = new StringWriter(sb);
             var xtw = new XmlTextWriter(sw) { Formatting = Formatting.Indented };

             try
             {
                 xd.WriteTo(xtw);
             }
             finally
             {
                 xtw.Close();
             }

             return sb.ToString();
         }
    }
}