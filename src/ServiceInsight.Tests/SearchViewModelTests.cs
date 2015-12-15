namespace ServiceInsight.Tests
{
    using ServiceInsight.Explorer.EndpointExplorer;
    using ServiceInsight.Models;
    using ServiceInsight.Search;
    using ServiceInsight.Startup;
    using NSubstitute;
    using NUnit.Framework;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.Settings;
    using Shouldly;

    [TestFixture]
    public class SearchViewModelTests
    {
        SearchBarViewModel ViewModel;
        CommandLineArgParser ArgParser;
        ISettingsProvider SettingProvider;

        [SetUp]
        public void TestInitialize()
        {
            ArgParser = Substitute.For<CommandLineArgParser>();
            SettingProvider = Substitute.For<ISettingsProvider>();
            ViewModel = new SearchBarViewModel(ArgParser, SettingProvider);
        }

        [Test]
        public void Should_enable_search_textbox_when_an_endpoint_is_selected()
        {
            ViewModel.Handle(new SelectedExplorerItemChanged(new AuditEndpointExplorerItem(new Endpoint())));

            ViewModel.SearchEnabled.ShouldBe(true);
        }

        [Test]
        public void Should_enable_search_textbox_when_service_control_node_is_selected()
        {
            ViewModel.Handle(new SelectedExplorerItemChanged(new ServiceControlExplorerItem("http://localhost")));

            ViewModel.SearchEnabled.ShouldBe(true);
        }
    }
}