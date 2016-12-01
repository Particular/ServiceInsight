namespace ServiceInsight.Tests
{
    using System.Text;
    using Caliburn.Micro;
    using NSubstitute;
    using NUnit.Framework;
    using ServiceInsight.MessageViewers.XmlViewer;
    using ServiceInsight.Models;
    using ServiceInsight.ServiceControl;
    using Shouldly;

    [TestFixture]
    public class XmlViewerViewModelTests
    {
        XmlMessageViewModel viewModel;
        IXmlMessageView view;
        const string TestMessage = "<?xml version=\"1.0\"?><Test title=\"test title\"/>";

        [SetUp]
        public void TestInitialize()
        {
            view = Substitute.For<IXmlMessageView>();
            viewModel = new XmlMessageViewModel();
            ((IActivate)viewModel).Activate();
        }

        [Test]
        public void Should_clear_message_body_when_selected_message_is_unselected()
        {
            viewModel.Display(new StoredMessage { Body = new PresentationBody { Text = TestMessage } });
            viewModel.Display(null);

            viewModel.SelectedMessage.ShouldBe(null);
        }

        [Test]
        public void Should_not_load_the_message_when_view_is_not_loaded()
        {
            viewModel.Display(new StoredMessage { Body = new PresentationBody { Text = TestMessage } });

            view.DidNotReceive().Display(Arg.Any<string>());
        }

        [Test]
        public void Should_not_throw_when_message_has_preamble_header()
        {
            ((IViewAware)viewModel).AttachView(view); //Activetes the view

            var messageBodyWithBOM = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble()) + TestMessage;

            var message = new StoredMessage
            {
                Body = new PresentationBody
                {
                    Text = messageBodyWithBOM
                }
            };

            Should.NotThrow(() => viewModel.Display(message));
            view.Received().Display(Arg.Is(messageBodyWithBOM));
        }
    }
}