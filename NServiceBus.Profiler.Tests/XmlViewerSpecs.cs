using System.Text;
using System.Xml;
using ExceptionHandler;
using Machine.Specifications;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core.MessageDecoders;
using NServiceBus.Profiler.XmlViewer;
using NSubstitute;

namespace NServiceBus.Profiler.Tests.XmlViewer
{
    [Subject("xml viewer")]
    public abstract class with_xml_viewer
    {
        protected static IXmlMessageViewModel ViewModel;
        protected static IXmlMessageView View;
        protected static IMessageDecoder<string> StringDecoder;
        protected static IMessageDecoder<XmlDocument> XmlDecoder;
        protected static IClipboard Clipboard;

        Establish context = () =>
        {
            StringDecoder = Substitute.For<IMessageDecoder<string>>();
            XmlDecoder = Substitute.For<IMessageDecoder<XmlDocument>>();
            Clipboard = Substitute.For<IClipboard>();
            View = Substitute.For<IXmlMessageView>();
            ViewModel = new XmlMessageViewModel(XmlDecoder, StringDecoder, Clipboard);
            ViewModel.Activate();
        };
    }

    public class when_displaying_message_as_xml_in_a_loaded_view : with_xml_viewer
    {
        protected static string TestMessage = "<?xml version=\"1.0\"?><Test title=\"test title\"/>";
        protected static XmlDocument Document;

        Establish context = () =>
        {
            Document = new XmlDocument();
            Document.LoadXml(TestMessage);
            XmlDecoder.Decode(Arg.Any<byte[]>()).Returns(Document);
            StringDecoder.Decode(Arg.Any<byte[]>()).Returns(TestMessage);

            ViewModel.AttachView(View, null);
        };

        Because of = () => ViewModel.SelectedMessage = new MessageBody {BodyRaw = Encoding.Default.GetBytes(TestMessage)};

        It should_display_the_message = () => View.Received(1).Display(Arg.Any<string>());
        It should_decode_the_message_to_xml_document = () => XmlDecoder.Received(1).Decode(Arg.Any<byte[]>());
    }

    public class when_selected_message_is_unselected : with_xml_viewer
    {
        protected static string TestMessage = "<?xml version=\"1.0\"?><Test title=\"test title\"/>";

        Because of = () =>
        {
            ViewModel.Handle(new MessageBodyLoadedEvent(new MessageBody { BodyRaw = Encoding.Default.GetBytes(TestMessage)}));
            ViewModel.Handle(new SelectedMessageChangedEvent(null));
        };

        It should_clear_message_body = () => ViewModel.SelectedMessage.ShouldBeNull();
    }

    public class when_view_is_not_loaded : with_xml_viewer
    {
        public static string TestMessage = "This is a test message content that is spread into four lines";

        Because of = () => ViewModel.Handle(new MessageBodyLoadedEvent(new MessageBody { BodyRaw = Encoding.Default.GetBytes(TestMessage) }));

        It should_not_load_the_message = () => View.DidNotReceive().Display(Arg.Any<string>()); 
    }

    public class with_copy_message_context_menu_item : with_xml_viewer
    {
        Because of = () => ViewModel.SelectedMessage = new MessageBody();
        It should_be_possible_to_copy_the_selected_message = () => ViewModel.CanCopyMessageXml().ShouldBeTrue();
    }

    public class when_copying_message : with_xml_viewer
    {
        Establish context = () => ViewModel.SelectedMessage = new MessageBody();

        Because of = () => ViewModel.CopyMessageXml();

        It clipboard_should_have_message_content = () => Clipboard.Received().CopyTo(Arg.Any<string>());
    }
}