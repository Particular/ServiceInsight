﻿namespace ServiceInsight.Framework
{
    using System.Linq;

    public class TypeHumanizer
    {
        public static string ToName(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return null;
            }

            var clazz = type.Split(',').First();
            var objectName = clazz.Split('.').Last();
            objectName = objectName.Replace('+', '.');

            return objectName;
        }
    }
}