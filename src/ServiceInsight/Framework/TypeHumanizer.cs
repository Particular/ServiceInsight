namespace Particular.ServiceInsight.Desktop.Framework
{
    using System.Linq;

    class TypeHumanizer
    {
        public static string ToName(string type)
        {
            if (string.IsNullOrEmpty(type))
                return string.Empty;

            var clazz = type.Split(',').First();
            var objectName = clazz.Split('.').Last();
            objectName = objectName.Split('+').Last();

            return objectName;
        }
    }
}