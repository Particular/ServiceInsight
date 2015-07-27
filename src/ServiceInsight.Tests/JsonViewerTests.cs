namespace Particular.ServiceInsight.Tests
{
    using Caliburn.Micro;
    using Desktop.MessageViewers.JsonViewer;
    using Desktop.Models;
    using NSubstitute;
    using NUnit.Framework;
    using Particular.ServiceInsight.Desktop.ServiceControl;

    [TestFixture]
    public class JsonViewerTests
    {
        JsonMessageViewModel ViewModel;
        IJsonMessageView View;

        [SetUp]
        public void TestInitialize()
        {
            View = Substitute.For<IJsonMessageView>();
            ViewModel = new JsonMessageViewModel();
            ((IActivate)ViewModel).Activate();
        }

        [Test]
        public void should_display_json_message()
        {
            const string TestMessage = @"[{""$type"":""NSB.Messages.CRM.RegisterCustomer, NSB.Messages"",""Name"":""Hadi"",""Password"":""123456"",""EmailAddress"":""h.eskandari@gmail.com"",""RegistrationDate"":""2013-01-28T03:24:05.0546437Z""}]";

            ((IViewAware)ViewModel).AttachView(View);

            ViewModel.SelectedMessage = new MessageBody { Body = new PresentationBody { Text = TestMessage } }; 

            View.Received(1).Display(Arg.Any<string>());
        }
    }
}