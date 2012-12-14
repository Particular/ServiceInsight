using System.IO;
using System.Text;

namespace NServiceBus.Profiler.Common.ExtensionMethods
{
    public static class StreamHelper
    {
        public static byte[] GetAsBytes(this Stream stream)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        public static string GetAsString(this byte[] data)
        {
            return GetAsString(data, Encoding.Default);
        }

        public static string GetAsString(this byte[] data, Encoding encoding)
        {
            return encoding.GetString(data);
        }
    }
}