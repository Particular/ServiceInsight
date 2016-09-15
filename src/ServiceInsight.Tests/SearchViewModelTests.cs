namespace ServiceInsight.Tests
{
    using NSubstitute;
    using NUnit.Framework;
    using ServiceInsight.Explorer.EndpointExplorer;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.Settings;
    using ServiceInsight.Models;
    using ServiceInsight.Search;
    using ServiceInsight.Startup;
    using Shouldly;

    [TestFixture]
    public class SearchViewModelTests
    {
        SearchBarViewModel viewModel;
        CommandLineArgParser argParser;
        ISettingsProvider settingProvider;
        IRxEventAggregator eventAggregator;

        [SetUp]
        public void TestInitialize()
        {
            argParser = Substitute.For<CommandLineArgParser>();
            settingProvider = Substitute.For<ISettingsProvider>();
            eventAggregator = Substitute.For<IRxEventAggregator>();
            viewModel = new SearchBarViewModel(argParser, settingProvider, eventAggregator);
        }

        [Test]
        public void Should_enable_search_textbox_when_an_endpoint_is_selected()
        {
            viewModel.Handle(new SelectedExplorerItemChanged(new AuditEndpointExplorerItem(new Endpoint())));

            viewModel.SearchEnabled.ShouldBe(true);
        }

        [Test]
        public void Should_enable_search_textbox_when_service_control_node_is_selected()
        {
            viewModel.Handle(new SelectedExplorerItemChanged(new ServiceControlExplorerItem("http://localhost")));

            viewModel.SearchEnabled.ShouldBe(true);
        }
    }
}