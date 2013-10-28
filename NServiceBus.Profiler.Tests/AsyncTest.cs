using NServiceBus.Profiler.Tests.Helpers;
using NUnit.Framework;

namespace NServiceBus.Profiler.Tests
{
    public abstract class AsyncTestBase
    {
        protected AsyncHelper Async;

        [SetUp]
        public void TestInitializeBase()
        {
            Async = new AsyncHelper();
        }
    }
}