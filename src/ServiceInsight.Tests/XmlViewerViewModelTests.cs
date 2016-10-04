namespace ServiceInsight.Tests
{
    using Caliburn.Micro;
    using MessageViewers;
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
        IMessageView view;
        const string TestMessage = "<?xml version=\"1.0\"?><Test title=\"test title\"/>";

        [SetUp]
        public void TestInitialize()
        {
            view = Substitute.For<IMessageView>();
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
    }
}