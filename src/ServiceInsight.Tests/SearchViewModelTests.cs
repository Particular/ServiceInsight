namespace ServiceInsight.Tests
{
    using System;
    using System.Windows.Threading;
    using NSubstitute;
    using NUnit.Framework;
    using ServiceInsight.Explorer.EndpointExplorer;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.Settings;
    using ServiceInsight.Models;
    using ServiceInsight.Search;
    using ServiceInsight.Startup;
    using Shouldly;

    [TestFixture]
    public sealed class SearchViewModelTests : IDisposable
    {
        SearchBarViewModel viewModel;

        public SearchViewModelTests()
        {
            var argParser = Substitute.For<CommandLineArgParser>();
            var settingProvider = Substitute.For<ISettingsProvider>();

            // required for async command
            var dispatcher = Dispatcher.CurrentDispatcher;
            viewModel = new SearchBarViewModel(argParser, settingProvider);
        }

        public void Dispose()
        {
            Dispatcher.ExitAllFrames();
            viewModel.Dispose();
        }

        [Test]
        public void Should_enable_search_textbox_when_an_endpoint_is_selected()
        {
            var serviceControl = new ServiceControlExplorerItem("http://localhost:3333/api");
            var auditNode = new AuditEndpointExplorerItem(serviceControl, new Endpoint());
            viewModel.Handle(new SelectedExplorerItemChanged(auditNode));

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