namespace Particular.ServiceInsight.Tests
{
    using Caliburn.Micro;
    using Desktop.MessageViewers.XmlViewer;
    using Desktop.Models;
    using NSubstitute;
    using NUnit.Framework;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using Particular.ServiceInsight.Desktop.ServiceControl;
    using Shouldly;

    [TestFixture]
    public class XmlViewerViewModelTests
    {
        XmlMessageViewModel ViewModel;
        IXmlMessageView View;
        const string TestMessage = "<?xml version=\"1.0\"?><Test title=\"test title\"/>";

        [SetUp]
        public void TestInitialize()
        {
            View = Substitute.For<IXmlMessageView>();
            ViewModel = new XmlMessageViewModel();
            ((IActivate)ViewModel).Activate();
        }

        [Test]
        public void should_clear_message_body_when_selected_message_is_unselected()
        {
            ViewModel.Display(new StoredMessage { Body = new PresentationBody { Text = TestMessage } });
            ViewModel.Display(null);

            ViewModel.SelectedMessage.ShouldBe(null);
        }

        [Test]
        public void should_not_load_the_message_when_view_is_not_loaded()
        {
            ViewModel.Display(new StoredMessage { Body = new PresentationBody { Text = TestMessage } });

            View.DidNotReceive().Display(Arg.Any<string>());
        }
    }
}