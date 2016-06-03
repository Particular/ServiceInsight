namespace ServiceInsight.ExtensionMethods
{
    using System.IO;
    using System.Text;

    public static class StreamHelper
    {
        public static byte[] GetAsBytes(this Stream stream)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        public static string GetAsString(this Stream stream)
        {
            var lastPosition = stream.Position;

            try
            {
                stream.Position = 0;
                var result = Encoding.UTF8.GetString(GetAsBytes(stream));

                return result;
            }
            finally
            {
                stream.Position = lastPosition;
            }
        }

        public static Stream GetAsStream(this string someString)
        {
            if (string.IsNullOrWhiteSpace(someString))
            {
                return null;
            }

            var bytes = Encoding.UTF8.GetBytes(someString);
            return new MemoryStream(bytes);
        }

        public static string GetAsString(this byte[] data) => Encoding.UTF8.GetString(data);
    }
}