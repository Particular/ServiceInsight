namespace ServiceInsight.FunctionalTests.Services
{
    using System;

    public class NameGenerator : IAutoRegister
    {
        public string GetUniqueName(string prefix = "")
        {
            var uniqueIdentifier = Guid.NewGuid().ToString();
            return prefix + uniqueIdentifier;
        } 
    }
}