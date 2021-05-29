namespace ServiceInsight.Tests
{
    using System;
    using NUnit.Framework;
    using ServiceInsight.Startup;

    [TestFixture]
    public class PipeNameTests
    {
        [Test]
        public void Should_return_pipe_name_with_the_current_user_username()
        {
            Assert.AreEqual($"ServiceInsight-{Environment.UserName}", PipeName.Value);
        }
    }
}