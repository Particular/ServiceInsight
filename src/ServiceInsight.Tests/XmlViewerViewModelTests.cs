namespace Particular.ServiceInsight.Tests
{
    using System.Xml;
    using Caliburn.Micro;
    using Desktop;
    using Desktop.Core.MessageDecoders;
    using Desktop.Events;
    using Desktop.MessageViewers.XmlViewer;
    using Desktop.Models;
    using NSubstitute;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class XmlViewerViewModelTests
    {
        XmlMessageViewModel ViewModel;
        IXmlMessageView View;
        IContentDecoder<XmlDocument> XmlDecoder;
        const string TestMessage = "<?xml version=\"1.0\"?><Test title=\"test title\"/>";

        [SetUp]
        public void TestInitialize()
        {
            XmlDecoder = Substitute.For<IContentDecoder<XmlDocument>>();
            AppServices.Clipboard = Substitute.For<IClipboard>();
            View = Substitute.For<IXmlMessageView>();
            ViewModel = new XmlMessageViewModel(XmlDecoder);
            ((IActivate)ViewModel).Activate();
        }

        [Test]
        public void should_decode_message_as_xml_when_view_is_loaded()
        {
            XmlDecoder.Decode(Arg.Any<byte[]>()).Returns(new DecoderResult<XmlDocument>(GetDocument(TestMessage)));

            ((IViewAware)ViewModel).AttachView(View);
            ViewModel.SelectedMessage = new MessageBody { Body = TestMessage };

            View.Received(1).Display(Arg.Any<string>());
            XmlDecoder.Received(1).Decode(Arg.Any<byte[]>());
        }

        [Test]
        public void should_clear_message_body_when_selected_message_is_unselected()
        {
            ViewModel.Handle(new SelectedMessageChanged(new StoredMessage { Body = TestMessage }));
            ViewModel.Handle(new SelectedMessageChanged(null));

            ViewModel.SelectedMessage.ShouldBe(null);
        }

        [Test]
        public void should_not_load_the_message_when_view_is_not_loaded()
        {
            ViewModel.Handle(new SelectedMessageChanged(new StoredMessage { Body = TestMessage }));

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
            ViewModel.SelectedMessage = new MessageBody { Body = TestMessage };

            XmlDecoder.Decode(Arg.Any<byte[]>()).Returns(new DecoderResult<XmlDocument>(GetDocument(TestMessage)));

            ViewModel.CopyMessageXml();

            AppServices.Clipboard.Received().CopyTo(Arg.Any<string>());
        }

        static XmlDocument GetDocument(string content)
        {
            var document = new XmlDocument();
            document.LoadXml(content);
            return document;
        }
    }
}