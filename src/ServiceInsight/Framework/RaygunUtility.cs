namespace ServiceInsight.Framework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Mindscape.Raygun4Net;
    using ServiceInsight.ExtensionMethods;
    using ServiceInsight.Models;

    static class RaygunUtility
    {
        public static string LastServiceControlVersion { get; set; }

        public static RaygunClient GetClient()
        {
            var assemblyInfo = typeof(App).Assembly.GetAttribute<AssemblyInformationalVersionAttribute>();
            var client = new RaygunClient("uX5c/PiCVqF31xlEm3jShA==")
            {
                ApplicationVersion = assemblyInfo != null ? assemblyInfo.InformationalVersion : "Unknown Version"
            };
            client.AddWrapperExceptions(typeof(AggregateException));
            return client;
        }

        public static void SendError(RaygunClient client, Exception e)
        {
            var extraData = new Dictionary<string, object>();
            AddServiceControlVersion(extraData);

            client.SendInBackground(e, null, extraData);
        }

        public static void SendError(RaygunClient client, Exception e, StoredMessage storedMessage)
        {
            var extraData = StoredMessageToDictionary(storedMessage);
            AddServiceControlVersion(extraData);

            client.SendInBackground(e, null, extraData);
        }

        static void AddServiceControlVersion(IDictionary extraData)
        {
            if (!string.IsNullOrEmpty(LastServiceControlVersion))
            {
                extraData.Add("ServiceControlVersion", LastServiceControlVersion);
            }
        }

        static IDictionary StoredMessageToDictionary(StoredMessage storedMessage) => storedMessage.Headers.ToDictionary(h => h.Key, h => h.Value);
    }
}