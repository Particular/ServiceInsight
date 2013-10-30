using System;

namespace NServiceBus.Profiler.FunctionalTests.Infrastructure
{
    public class NameGenerator
    {
        public string GetUniqueName(string prefix = "")
        {
            var uniqueIdentifier = Guid.NewGuid().ToString();
            return prefix + uniqueIdentifier;
        } 
    }
}