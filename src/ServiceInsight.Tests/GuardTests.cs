namespace Particular.ServiceInsight.Tests
{
    using System;
    using Desktop;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class GuardTests
    {
        GuardTest GuardTest;

        [SetUp]
        public void TestInitialize()
        {
            GuardTest = new GuardTest();
        }


        [Test]
        public void throws_when_variable_is_null_when_null_passed_in()
        {
            Should.Throw<ArgumentNullException>(() => GuardTest.Run(null));
        }

        [Test]
        public void throws_when_variable_is_empty_string_when_empty_string_passed_in()
        {
            Should.Throw<ArgumentException>(() => GuardTest.Run(string.Empty));
        }

    }

    public class GuardTest
    {
        public const string ErrorMessage = "Something is wrong";

        public void Run(string testName)
        {
            Guard.NotNull(() => testName, testName);
            Guard.NotNullOrEmpty(() => testName, testName);
        }

    }

}