namespace ServiceInsight.Tests
{
    using ConversationsData;
    using NUnit.Framework;

    [TestFixture]
    class SequenceDiagramModelCreatorTestsForJson4 : SequenceDiagramModelCreatorTestsFromJson
    {
        public SequenceDiagramModelCreatorTestsForJson4()
            : base("json4.json")
        {
        }

        [Test]
        public void ShouldHaveFourEndpoints()
        {
            Assert.That(result, Has.Count.EqualTo(4));
            Assert.Multiple(() =>
            {
                Assert.That(result[0].Name, Is.EqualTo("Provisioning.CustomerPortal"));
                Assert.That(result[1].Name, Is.EqualTo("Provisioning.UserService.Orchestrator"));
                Assert.That(result[2].Name, Is.EqualTo("Provisioning.Audit.Orchestrator"));
                Assert.That(result[3].Name, Is.EqualTo("Provisioning.Communication.Orchestrator"));
            });
        }

        [Test]
        public void ShouldHaveStartOfConversationAsFirstHandler()
        {
            Assert.That(ModelCreator.Handlers[0].ID, Is.EqualTo(SequenceDiagram.ModelCreator.ConversationStartHandlerName));
        }

        [Test]
        public void ShouldHaveHandlersProperlyOrdered()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ModelCreator.Handlers[0].Name, Is.EqualTo(null));
                Assert.That(ModelCreator.Handlers[1].Name, Is.EqualTo("LogCRMAudit"));
                Assert.That(ModelCreator.Handlers[2].Name, Is.EqualTo("UserSignupRequestAccepted"));
                Assert.That(ModelCreator.Handlers[3].Name, Is.EqualTo("CreateCRMAccountRequest"));
                Assert.That(ModelCreator.Handlers[4].Name, Is.EqualTo("CreateCRMContactRequest"));
                Assert.That(ModelCreator.Handlers[5].Name, Is.EqualTo("ADUserCreated"));
                Assert.That(ModelCreator.Handlers[6].Name, Is.EqualTo("CreateCRMAccountResponse"));
                Assert.That(ModelCreator.Handlers[7].Name, Is.EqualTo("CreateCRMContactCreatedResponse"));
                Assert.That(ModelCreator.Handlers[8].Name, Is.EqualTo("StoreUserActivationKeyInCache"));
                Assert.That(ModelCreator.Handlers[9].Name, Is.EqualTo("LinkCRMAccountAndContactRequest"));
                Assert.That(ModelCreator.Handlers[10].Name, Is.EqualTo("LinkCRMAccountAndContactResponse"));
                Assert.That(ModelCreator.Handlers[11].Name, Is.EqualTo("NewAccountCreated"));
                Assert.That(ModelCreator.Handlers[12].Name, Is.EqualTo("SendAddAccountEmailRequestAccepted"));
            });
        }
    }
}