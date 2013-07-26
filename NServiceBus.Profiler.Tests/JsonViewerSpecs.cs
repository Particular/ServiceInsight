using System.Text;
using Machine.Specifications;
using NServiceBus.Profiler.Desktop.Core.MessageDecoders;
using NServiceBus.Profiler.Desktop.MessageViewers.JsonViewer;
using NServiceBus.Profiler.Desktop.Models;
using NSubstitute;

namespace NServiceBus.Profiler.Tests.JsonViewer
{
    [Subject("Json viewer")]
    public abstract class with_json_viewer
    {
        protected static IJsonMessageViewModel ViewModel;
        protected static IContentDecoder<string> Decoder;
        protected static IJsonMessageView View;
            
        Establish context = () =>
        {
            View = Substitute.For<IJsonMessageView>();
            Decoder = Substitute.For<IContentDecoder<string>>();
            ViewModel = new JsonMessageViewModel(Decoder);
            ViewModel.Activate();
        };
    }

    public class when_displaying_messages_serialized_as_json : with_json_viewer
    {
        protected static string TestMessage = @"[{""$type"":""NSB.Messages.CRM.RegisterCustomer, NSB.Messages"",""Name"":""Hadi"",""Password"":""123456"",""EmailAddress"":""h.eskandari@gmail.com"",""RegistrationDate"":""2013-01-28T03:24:05.0546437Z""}]";

        Establish context = () =>
        {
            Decoder.Decode(Arg.Any<byte[]>()).Returns(new DecoderResult<string>(TestMessage));
            ViewModel.AttachView(View, null);
        };

        Because of = () => ViewModel.SelectedMessage = new MessageBody { BodyRaw = Encoding.Default.GetBytes(TestMessage) };

        It displays_the_message_as_json_object = () => View.Received(1).Display(Arg.Any<string>());
    }
}