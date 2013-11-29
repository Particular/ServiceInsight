using System.Linq;
using System.Text;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.MessageViewers.HexViewer;
using NServiceBus.Profiler.Desktop.Models;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace NServiceBus.Profiler.Tests
{
    [TestFixture]
    public class HexViewerTests
    {
        private IHexContentViewModel ViewModel;
        private IHexContentView View;
        const string TestMessage = "This is a test message content that is spread into four lines";

        [SetUp]
        public void TestInitialize()
        {
            View = Substitute.For<IHexContentView>();
            ViewModel = new HexContentViewModel();
        }

        [Test]
        public void should_display_the_message_when_view_is_loaded()
        {
            ViewModel.AttachView(View, null);
            ((IActivate)ViewModel).Activate();

            ViewModel.CurrentContent = Encoding.Default.GetBytes(TestMessage);

            ViewModel.HexParts.ShouldNotBeEmpty();
            ViewModel.HexParts.Count.ShouldBe(4);
        }

        [Test]
        public void should_not_load_the_message_when_view_is_not_loaded()
        {
            ViewModel.CurrentContent = Encoding.Default.GetBytes(TestMessage);

            ViewModel.HexParts.ShouldBeEmpty();
        }

        [Test]
        public void should_clear_the_view_when_message_is_unselected()
        {
            ViewModel.Handle(new MessageBodyLoaded(new MessageBody { BodyRaw = Encoding.Default.GetBytes(TestMessage) }));

            ViewModel.Handle(new SelectedMessageChanged(null));

            ViewModel.HexParts.Count.ShouldBe(0);
            ViewModel.CurrentContent.ShouldBe(null);
        }

        [Test]
        public void should_convert_special_characters_to_dot()
        {
            const string messageWithSpecialChars = "This is a multiline test\rmessage content\tthat is spread into four lines";

            ViewModel.AttachView(View, null);
            ((IActivate)ViewModel).Activate();

            ViewModel.Handle(new MessageBodyLoaded(new MessageBody { BodyRaw = Encoding.Default.GetBytes(messageWithSpecialChars) }));

            ViewModel.HexParts.ShouldNotBeEmpty();
            ViewModel.HexParts[1].Numbers.Count(n => n.Text == ".").ShouldBe(1);
            ViewModel.HexParts[2].Numbers.Count(n => n.Text == ".").ShouldBe(1);
        }
    }
}