using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Mindscape.Raygun4Net;
using ServiceInsight.ExtensionMethods;
using ServiceInsight.Models;

namespace ServiceInsight.Framework
{
    static class RaygunUtility
    {
        public static RaygunClient GetClient()
        {
            var assemblyInfo = typeof(App).Assembly.GetAttribute<AssemblyInformationalVersionAttribute>();
            var client = new RaygunClient("uX5c/PiCVqF31xlEm3jShA==")
            {
                ApplicationVersion = assemblyInfo != null ? assemblyInfo.InformationalVersion : "Unknown Version"
            };
            return client;
        }

        public static void SendError(RaygunClient client, Exception e)
        {
            client.SendInBackground(e);
        }

        public static void SendError(RaygunClient client, Exception e, StoredMessage storedMessage)
        {
            client.SendInBackground(e, null, StoredMessageToDictionary(storedMessage));
        }

        private static IDictionary StoredMessageToDictionary(StoredMessage storedMessage)
        {
            return storedMessage.Headers.ToDictionary(h => h.Key, h => h.Value);
        }
    }
}