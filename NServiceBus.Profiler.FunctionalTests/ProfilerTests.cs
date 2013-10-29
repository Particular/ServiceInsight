using System;

namespace NServiceBus.Profiler.FunctionalTests
{
    public abstract class ProfilerTests : TestBase
    {
        protected string GetUniqueName(string prefix = "")
        {
            var uniqueIdentifier = Guid.NewGuid().ToString();
            return prefix + uniqueIdentifier;
        }
    }
}