using System.Text;
using NServiceBus.Profiler.Desktop.Core.MessageDecoders;
using NServiceBus.Profiler.Desktop.MessageViewers.JsonViewer;
using NServiceBus.Profiler.Desktop.Models;
using NSubstitute;
using NUnit.Framework;

namespace NServiceBus.Profiler.Tests
{
    [TestFixture]
    public class JsonViewerTests
    {
        private IJsonMessageViewModel ViewModel;
        private IContentDecoder<string> Decoder;
        private IJsonMessageView View;

        [SetUp]
        public void TestInitialize()
        {
            View = Substitute.For<IJsonMessageView>();
            Decoder = Substitute.For<IContentDecoder<string>>();
            ViewModel = new JsonMessageViewModel(Decoder);
            ViewModel.Activate();
        }

        [Test]
        public void should_display_json_message()
        {
            const string TestMessage = @"[{""$type"":""NSB.Messages.CRM.RegisterCustomer, NSB.Messages"",""Name"":""Hadi"",""Password"":""123456"",""EmailAddress"":""h.eskandari@gmail.com"",""RegistrationDate"":""2013-01-28T03:24:05.0546437Z""}]";

            Decoder.Decode(Arg.Any<byte[]>()).Returns(new DecoderResult<string>(TestMessage));
            ViewModel.AttachView(View, null);

            ViewModel.SelectedMessage = new MessageBody { BodyRaw = Encoding.Default.GetBytes(TestMessage) };

            View.Received(1).Display(Arg.Any<string>());
        }
    }
}