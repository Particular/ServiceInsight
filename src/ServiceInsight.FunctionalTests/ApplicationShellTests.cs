namespace Particular.ServiceInsight.FunctionalTests
{
    using NUnit.Framework;

    [TestFixture]
    public class ApplicationShellTests : TestBase
    {
        [Test]
        public void Application_main_window_is_displayed_on_startup()
        {
            Assert.NotNull(MainWindow);
        }
    }
}