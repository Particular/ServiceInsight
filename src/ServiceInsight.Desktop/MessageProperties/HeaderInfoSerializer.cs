using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using DevExpress.Xpf.Core;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.MessageProperties
{
    public class HeaderInfoSerializer : IHeaderInfoSerializer
    {
        public string Serialize(IList<HeaderInfo> headers)
        {
            var serializer = new XmlSerializer(typeof(HeaderInfo[]));
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, headers.ToArray());
                var content = stream.ReadString();

                return content;
            }
        }
    }

    public interface IHeaderInfoSerializer
    {
        string Serialize(IList<HeaderInfo> headers);
    }
}