using System.Text;
using Caliburn.PresentationFramework.Screens;
using Machine.Specifications;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.HexViewer;
using NSubstitute;
using System.Linq;

namespace NServiceBus.Profiler.Tests.HexViewer
{
    [Subject("hex viewer")]
    public abstract class with_hex_viewer
    {
        protected static IHexContentViewModel ViewModel;
        protected static IHexContentView View;

        Establish context = () =>
        {
            View = Substitute.For<IHexContentView>();
            ViewModel = new HexContentViewModel();
        };
    }

    public class when_displaying_message_as_hex_in_a_loaded_view : with_hex_viewer
    {
        public static string TestMessage = "This is a test message content that is spread into four lines";

        Establish context = () =>
        {
            ViewModel.AttachView(View, null);
            ViewModel.Activate();
        };

        Because of = () => ViewModel.CurrentContent = Encoding.Default.GetBytes(TestMessage);

        It should_display_the_message = () => ViewModel.HexParts.ShouldNotBeEmpty();
        It should_display_each_sixteen_bytes_in_one_line = () => ViewModel.HexParts.Count.ShouldEqual(4);
    }

    public class when_view_is_not_loaded : with_hex_viewer
    {
        public static string TestMessage = "This is a test message content that is spread into four lines";

        Because of = () =>
        {
            ViewModel.CurrentContent = Encoding.Default.GetBytes(TestMessage);
        };

        It should_not_load_the_message = () => ViewModel.HexParts.ShouldBeEmpty();
    }

    public class when_selected_message_is_unselected : with_hex_viewer
    {
        protected static string TestMessage = "This is a test message content that is spread into four lines";

        Because of = () =>
        {
            ViewModel.Handle(new MessageBodyLoadedEvent(new MessageBody { BodyRaw = Encoding.Default.GetBytes(TestMessage) }));
            ViewModel.Handle(new SelectedMessageChangedEvent(null));
        };

        It should_clear_hex_view = () => ViewModel.HexParts.Count.ShouldEqual(0);
        It should_clear_the_selected_message = () => ViewModel.CurrentContent.ShouldBeNull();
    }

    public class converting_multiline_content : with_hex_viewer
    {
        public static string TestMessage = "This is a multiline test\rmessage content\tthat is spread into four lines";

        Establish context = () =>
        {
            ViewModel.AttachView(View, null);
            ((IActivate)ViewModel).Activate();
        };

        Because of = () => ViewModel.Handle(new MessageBodyLoadedEvent(new MessageBody
        {
            BodyRaw = Encoding.Default.GetBytes(TestMessage)
        }));

        It should_display_the_message = () => ViewModel.HexParts.ShouldNotBeEmpty();
        It should_convert_carriage_return_to_dot_char = () => ViewModel.HexParts[1].Numbers.Count(n => n.Text == ".").ShouldEqual(1);
        It should_convert_tab_to_dot_char = () => ViewModel.HexParts[2].Numbers.Count(n => n.Text == ".").ShouldEqual(1);
    }
}