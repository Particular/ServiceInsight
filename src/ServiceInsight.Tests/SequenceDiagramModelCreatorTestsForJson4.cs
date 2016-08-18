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
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("Provisioning.CustomerPortal", result[0].Name);
            Assert.AreEqual("Provisioning.UserService.Orchestrator", result[1].Name);
            Assert.AreEqual("Provisioning.Audit.Orchestrator", result[2].Name);
            Assert.AreEqual("Provisioning.Communication.Orchestrator", result[3].Name);
        }

        [Test]
        public void ShouldHaveStartOfConversationAsFirstHandler()
        {
            Assert.AreEqual(SequenceDiagram.ModelCreator.ConversationStartHandlerName, ModelCreator.Handlers[0].ID);
        }

        [Test]
        public void ShouldHaveHandlersProperlyOrdered()
        {
            Assert.AreEqual(null, ModelCreator.Handlers[0].Name);
            Assert.AreEqual("LogCRMAudit", ModelCreator.Handlers[1].Name);
            Assert.AreEqual("UserSignupRequestAccepted", ModelCreator.Handlers[2].Name);
            Assert.AreEqual("CreateCRMAccountRequest", ModelCreator.Handlers[3].Name);
            Assert.AreEqual("CreateCRMContactRequest", ModelCreator.Handlers[4].Name);
            Assert.AreEqual("ADUserCreated", ModelCreator.Handlers[5].Name);
            Assert.AreEqual("CreateCRMAccountResponse", ModelCreator.Handlers[6].Name);
            Assert.AreEqual("CreateCRMContactCreatedResponse", ModelCreator.Handlers[7].Name);
            Assert.AreEqual("StoreUserActivationKeyInCache", ModelCreator.Handlers[8].Name);
            Assert.AreEqual("LinkCRMAccountAndContactRequest", ModelCreator.Handlers[9].Name);
            Assert.AreEqual("LinkCRMAccountAndContactResponse", ModelCreator.Handlers[10].Name);
            Assert.AreEqual("NewAccountCreated", ModelCreator.Handlers[11].Name);
            Assert.AreEqual("SendAddAccountEmailRequestAccepted", ModelCreator.Handlers[12].Name);
        }
    }
}