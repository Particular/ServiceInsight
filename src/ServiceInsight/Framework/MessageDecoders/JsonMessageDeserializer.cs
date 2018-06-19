namespace ServiceInsight.Framework.MessageDecoders
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Caliburn.Micro;
    using RestSharp;
    using RestSharp.Deserializers;
    using RestSharp.Extensions;

    public class JsonMessageDeserializer : IDeserializer
    {
        public string RootElement { get; set; }

        public string Namespace { get; set; }

        public string DateFormat { get; set; }

        public CultureInfo Culture { get; set; }

        public bool TruncateLargeLists { get; set; }

        public JsonMessageDeserializer()
        {
            Culture = CultureInfo.InvariantCulture;
        }

        public T Deserialize<T>(IRestResponse response)
        {
            var target = Activator.CreateInstance<T>();

            if (target is IList)
            {
                var objType = target.GetType();

                if (RootElement.HasValue())
                {
                    var root = FindRoot(response.Content);
                    target = (T)BuildList(objType, root);
                }
                else
                {
                    var data = SimpleJson.DeserializeObject(response.Content);
                    target = (T)BuildList(objType, data);
                }
            }
            else if (target is IDictionary)
            {
                var root = FindRoot(response.Content);
                target = (T)BuildDictionary(target.GetType(), root);
            }
            else
            {
                var root = FindRoot(response.Content);
                Map(target, (IDictionary<string, object>)root);
            }

            return target;
        }

        object FindRoot(string content)
        {
            var data = (IDictionary<string, object>)SimpleJson.DeserializeObject(content);
            if (RootElement.HasValue() && data.ContainsKey(RootElement))
            {
                return data[RootElement];
            }
            return data;
        }

        void Map(object target, IDictionary<string, object> data)
        {
            var notifiableTarget = target as INotifyPropertyChangedEx;
            if (notifiableTarget != null)
            {
                notifiableTarget.IsNotifying = false;
            }

            var objType = target.GetType();
            var props = objType.GetProperties().Where(p => p.CanWrite).ToList();

            foreach (var prop in props)
            {
                var type = prop.PropertyType;

                string name;

                var attributes = prop.GetCustomAttributes(typeof(DeserializeAsAttribute), false);
                if (attributes.Length > 0)
                {
                    var attribute = (DeserializeAsAttribute)attributes[0];
                    name = attribute.Name;
                }
                else
                {
                    name = prop.Name;
                }

                var actualName = name.GetNameVariants(Culture).FirstOrDefault(data.ContainsKey);
                var value = actualName != null ? data[actualName] : null;

                if (value == null)
                {
                    continue;
                }

                prop.SetValue(target, ConvertValue(type, value), null);
            }

            if (notifiableTarget != null)
            {
                notifiableTarget.IsNotifying = true;
            }
        }

        IDictionary BuildDictionary(Type type, object parent)
        {
            var dict = (IDictionary)Activator.CreateInstance(type);
            var valueType = type.GetGenericArguments()[1];
            foreach (var child in (IDictionary<string, object>)parent)
            {
                var key = child.Key;
                var item = ConvertValue(valueType, child.Value);
                dict.Add(key, item);
            }

            return dict;
        }

        IList BuildList(Type type, object parent)
        {
            var list = (IList)Activator.CreateInstance(type);
            var listType = type.GetInterfaces().First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
            var itemType = listType.GetGenericArguments()[0];

            var elements = parent as IList;
            if (elements != null)
            {
                if (elements.Count > 200 && TruncateLargeLists)
                {
                    var missingJObject = new JsonObject
                    {
                        ["missing_data"] = true
                    };

                    elements = elements.Cast<object>().Take(100)
                        .Concat(new[] { missingJObject })
                        .Concat(elements.Cast<object>().Skip(elements.Count - 100))
                        .ToList();
                }

                foreach (var element in elements)
                {
                    if (itemType.IsPrimitive)
                    {
                        var value = element.ToString();
                        list.Add(value.ChangeType(itemType, Culture));
                    }
                    else if (itemType == typeof(string))
                    {
                        if (element == null)
                        {
                            list.Add(null);
                            continue;
                        }

                        list.Add(element.ToString());
                    }
                    else
                    {
                        if (element == null)
                        {
                            list.Add(null);
                            continue;
                        }

                        var item = ConvertValue(itemType, element);
                        list.Add(item);
                    }
                }
            }
            else
            {
                list.Add(ConvertValue(itemType, parent));
            }
            return list;
        }

        object ConvertValue(Type type, object value)
        {
            var stringValue = Convert.ToString(value, Culture);

            // check for nullable and extract underlying type
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // Since the type is nullable and no value is provided return null
                if (string.IsNullOrEmpty(stringValue))
                {
                    return null;
                }

                type = type.GetGenericArguments()[0];
            }

            if (type == typeof(object) && value != null)
            {
                type = value.GetType();
            }

            if (type.IsPrimitive)
            {
                return value.ChangeType(type, Culture);
            }
            if (type.IsEnum)
            {
                return type.FindEnumValue(stringValue, Culture);
            }
            if (type == typeof(Uri))
            {
                return new Uri(stringValue, UriKind.RelativeOrAbsolute);
            }
            if (type == typeof(string))
            {
                return stringValue;
            }
            if (type == typeof(DateTime) || type == typeof(DateTimeOffset))
            {
                DateTime dt;
                if (DateFormat.HasValue())
                {
                    dt = DateTime.ParseExact(stringValue, DateFormat, Culture);
                }
                else
                {
                    // try parsing instead
                    dt = stringValue.ParseJsonDate(Culture);
                }

                if (type == typeof(DateTime))
                {
                    return dt;
                }
                if (type == typeof(DateTimeOffset))
                {
                    return (DateTimeOffset)dt;
                }
            }
            else if (type == typeof(decimal))
            {
                if (value is double)
                {
                    return (decimal)((double)value);
                }

                return decimal.Parse(stringValue, Culture);
            }
            else if (type == typeof(byte[]))
            {
                return ConvertByteArray(stringValue);
            }
            else if (type == typeof(Guid))
            {
                return string.IsNullOrEmpty(stringValue) ? Guid.Empty : new Guid(stringValue);
            }
            else if (type == typeof(TimeSpan))
            {
                return TimeSpan.Parse(stringValue);
            }
            else if (type.IsGenericType)
            {
                var genericTypeDef = type.GetGenericTypeDefinition();
                if (genericTypeDef == typeof(List<>))
                {
                    return BuildList(type, value);
                }
                if (genericTypeDef == typeof(Dictionary<,>))
                {
                    var keyType = type.GetGenericArguments()[0];

                    // only supports Dict<string, T>()
                    if (keyType == typeof(string))
                    {
                        return BuildDictionary(type, value);
                    }
                }
                else
                {
                    // nested property classes
                    return CreateAndMap(type, value);
                }
            }
            else
            {
                // nested property classes
                return CreateAndMap(type, value);
            }

            return null;
        }

        object CreateAndMap(Type type, object element)
        {
            var instance = Activator.CreateInstance(type);

            Map(instance, (IDictionary<string, object>)element);

            return instance;
        }

        byte[] ConvertByteArray(string str) => str.Length > 0 ? Encoding.UTF8.GetBytes(str) : new byte[0];
    }
}