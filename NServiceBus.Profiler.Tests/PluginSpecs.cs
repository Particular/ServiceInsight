using System.Collections.Generic;
using Caliburn.PresentationFramework.Screens;
using Machine.Specifications;
using System.Linq;
using NServiceBus.Profiler.Common;
using NServiceBus.Profiler.Common.Plugins;

namespace NServiceBus.Profiler.Tests.Plugins
{
    [Subject("plugins")]
    public class when_having_plugin_information
    {
        protected static TestPlugin Plugin;
        protected static PluginContextMenu MenuItem;

        Establish context = () => Plugin = new TestPlugin();

        Because of = () => MenuItem = Plugin.ContextMenuItems.First();

        It should_be_possible_to_invoke_the_menu_item = () => MenuItem.Command.CanExecute("").ShouldBeTrue();
        It should_have_default_ordering_if_not_specified = () => MenuItem.Order.ShouldEqual(100);
        It should_have_no_image_by_default = () => MenuItem.Image.ShouldBeNull();
        It should_have_a_name = () => MenuItem.Name.ShouldEqual("Test");
        It should_have_a_display_name = () => MenuItem.DisplayName.ShouldEqual("Testing Action");

        It should_allow_invoking_the_menu_item_by_default = () =>
        {
            MenuItem.Command.Execute("");
            Plugin.ItemInvoked.ShouldBeTrue();
        };
    }

    public class TestPlugin : Screen, IPlugin
    {
        public TestPlugin()
        {
            ContextMenuItems = new List<PluginContextMenu>();
            ContextMenuItems.Add(new PluginContextMenu("Test", new RelayCommand(InvokeMenuitem))
            {
                DisplayName = "Testing Action",
            });
            TabOrder = 0;
        }

        private void InvokeMenuitem()
        {
            ItemInvoked = true;
        }

        public bool ItemInvoked { get; set; }

        public IList<PluginContextMenu> ContextMenuItems { get; private set; }
        public int TabOrder { get; private set; }
    }
}