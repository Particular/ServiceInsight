namespace Particular.ServiceInsight.Tests
{
    using Desktop.MessageViewers.JsonViewer;
    using Desktop.Models;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class JsonViewerTests
    {
        private IJsonMessageViewModel ViewModel;
        private IJsonMessageView View;

        [SetUp]
        public void TestInitialize()
        {
            View = Substitute.For<IJsonMessageView>();
            ViewModel = new JsonMessageViewModel();
            ViewModel.Activate();
        }

        [Test]
        public void should_display_json_message()
        {
            const string TestMessage = @"[{""$type"":""NSB.Messages.CRM.RegisterCustomer, NSB.Messages"",""Name"":""Hadi"",""Password"":""123456"",""EmailAddress"":""h.eskandari@gmail.com"",""RegistrationDate"":""2013-01-28T03:24:05.0546437Z""}]";

            ViewModel.AttachView(View, null);

            ViewModel.SelectedMessage = new MessageBody { Body = TestMessage };

            View.Received(1).Display(Arg.Any<string>());
        }
    }
}