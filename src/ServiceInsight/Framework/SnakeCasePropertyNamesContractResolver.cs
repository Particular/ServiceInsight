namespace ServiceInsight.Framework
{
    using System.Collections.Generic;
    using System.Text;
    using Newtonsoft.Json.Serialization;

    public class SnakeCasePropertyNamesContractResolver : DeliminatorSeparatedPropertyNamesContractResolver
    {
        public SnakeCasePropertyNamesContractResolver()
            : base('_') { }
    }

    public class DeliminatorSeparatedPropertyNamesContractResolver : DefaultContractResolver
    {
        private readonly string separator;

        protected DeliminatorSeparatedPropertyNamesContractResolver(char separator)
            : base(true)
        {
            this.separator = separator.ToString();
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            var parts = new List<string>();
            var currentWord = new StringBuilder();

            foreach (var c in propertyName)
            {
                if (char.IsUpper(c) && currentWord.Length > 0)
                {
                    parts.Add(currentWord.ToString());
                    currentWord.Clear();
                }
                currentWord.Append(char.ToLower(c));
            }

            if (currentWord.Length > 0)
            {
                parts.Add(currentWord.ToString());
            }

            return string.Join(separator, parts.ToArray());
        }
    }
}