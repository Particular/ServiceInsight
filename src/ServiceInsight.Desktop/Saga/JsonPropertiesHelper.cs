namespace Particular.ServiceInsight.Desktop.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    class JsonPropertiesHelper
    {
        private static IList<string> standardKeys = new List<string> { "$type", "Id", "Originator", "OriginalMessageId" };

        public static IList<KeyValuePair<string, string>> ProcessValues(string stateAfterChange)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(stateAfterChange)
                .Where(m => !standardKeys.Any(s => s == m.Key))
                .Select(f => new KeyValuePair<string, string>(f.Key, f.Value == null ? string.Empty : f.Value.ToString()))
                .ToList();
        }

        public static IList<KeyValuePair<string, string>> ProcessValues(string stateAfterChange, Func<string, string> cleanup)
        {
            return ProcessValues(cleanup(stateAfterChange));
        }
    }
}
