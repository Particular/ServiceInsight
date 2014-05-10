namespace Particular.ServiceInsight.Tests.Helpers
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public class BinaryObjectCloner
    {
        public static T Clone<T>(T instance)
        {
            var serializer = new BinaryFormatter();
            var ms = new MemoryStream();

            serializer.Serialize(ms, instance);
            ms.Position = 0;

            return (T)serializer.Deserialize(ms);
        }
    }
}