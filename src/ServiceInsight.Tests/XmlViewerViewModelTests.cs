using System.Text;
using System.Xml;
using ExceptionHandler;
using NServiceBus.Profiler.Desktop.Core.MessageDecoders;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.MessageViewers.XmlViewer;
using NServiceBus.Profiler.Desktop.Models;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace NServiceBus.Profiler.Tests
{
    [TestFixture]
    public class XmlViewerViewModelTests
    {
        private IXmlMessageViewModel ViewModel;
        private IXmlMessageView View;
        private IContentDecoder<string> StringDecoder;
        private IContentDecoder<XmlDocument> XmlDecoder;
        private IClipboard Clipboard;
        const string TestMessage = "<?xml version=\"1.0\"?><Test title=\"test title\"/>";

        [SetUp]
        public void TestInitialize()
        {
            StringDecoder = Substitute.For<IContentDecoder<string>>();
            XmlDecoder = Substitute.For<IContentDecoder<XmlDocument>>();
            Clipboard = Substitute.For<IClipboard>();
            View = Substitute.For<IXmlMessageView>();
            ViewModel = new XmlMessageViewModel(XmlDecoder, StringDecoder, Clipboard);
            ViewModel.Activate();
        }

        [Test]
        public void should_decode_message_as_xml_when_view_is_loaded()
        {
            var document = new XmlDocument();
            document.LoadXml(TestMessage);
            XmlDecoder.Decode(Arg.Any<byte[]>()).Returns(new DecoderResult<XmlDocument>(document));
            StringDecoder.Decode(Arg.Any<byte[]>()).Returns(new DecoderResult<string>(TestMessage));

            ViewModel.AttachView(View, null);
            ViewModel.SelectedMessage = new MessageBody { BodyRaw = Encoding.Default.GetBytes(TestMessage) };

            View.Received(1).Display(Arg.Any<string>());
            XmlDecoder.Received(1).Decode(Arg.Any<byte[]>());
        }

        [Test]
        public void should_clear_message_body_when_selected_message_is_unselected()
        {
            ViewModel.Handle(new SelectedMessageChanged(new StoredMessage { BodyRaw = Encoding.Default.GetBytes(TestMessage) }));
            ViewModel.Handle(new SelectedMessageChanged(null));

            ViewModel.SelectedMessage.ShouldBe(null);
        }

        [Test]
        public void should_not_load_the_message_when_view_is_not_loaded()
        {
            ViewModel.Handle(new SelectedMessageChanged(new StoredMessage { BodyRaw = Encoding.Default.GetBytes(TestMessage) }));

            View.DidNotReceive().Display(Arg.Any<string>()); 
        }

        [Test]
        public void should_be_possible_to_copy_the_selected_message_with_copy_message_context_menu_item()
        {
            ViewModel.SelectedMessage = new MessageBody();

            ViewModel.CanCopyMessageXml().ShouldBe(true);
        }

        [Test]
        public void clipboard_should_have_message_content_when_copying_message()
        {
            ViewModel.SelectedMessage = new MessageBody { BodyRaw = Encoding.UTF8.GetBytes(TestMessage) };
            StringDecoder.Decode(Arg.Any<byte[]>()).Returns(new DecoderResult<string>(TestMessage));
            
            ViewModel.CopyMessageXml();

            Clipboard.Received().CopyTo(Arg.Any<string>());
        }
    }
}