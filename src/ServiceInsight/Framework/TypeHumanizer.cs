namespace ServiceInsight.Framework
{
    using System.Linq;

    class TypeHumanizer
    {
        public static string ToName(string type)
        {
            if (string.IsNullOrEmpty(type))
                return null;

            var clazz = type.Split(',').First();
            var objectName = clazz.Split('.').Last();
            objectName = objectName.Split('+').Last();

            return objectName;
        }
    }
}