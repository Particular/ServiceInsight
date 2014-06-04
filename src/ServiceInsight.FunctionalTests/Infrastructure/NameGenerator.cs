namespace Particular.ServiceInsight.FunctionalTests.Infrastructure
{
    using System;

    public class NameGenerator
    {
        public string GetUniqueName(string prefix = "")
        {
            var uniqueIdentifier = Guid.NewGuid().ToString();
            return prefix + uniqueIdentifier;
        } 
    }
}