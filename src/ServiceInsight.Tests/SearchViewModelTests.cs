using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.Desktop.Search;
using NServiceBus.Profiler.Desktop.Startup;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace NServiceBus.Profiler.Tests
{
    using Desktop.Core.Settings;

    [TestFixture]
    public class SearchViewModelTests
    {
        private ISearchBarViewModel ViewModel;
        private ICommandLineArgParser ArgParser;
        private ISettingsProvider SettingProvider;

        [SetUp]
        public void TestInitialize()
        {
            ArgParser = Substitute.For<ICommandLineArgParser>();
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