namespace ServiceInsight.Tests.Framework
{
    using NUnit.Framework;
    using ServiceInsight.Framework.Attachments;

    [TestFixture]
    public class Attachments
    {
        class TestAttachment : Attachment<object>
        {
            readonly object original;

            public TestAttachment(object original)
            {
                this.original = original;
            }

            protected override void OnAttach()
            {
                Assert.AreEqual(original, instance);
            }
        }

        [Test]
        public void Attachment_sets_instance_before_onattach()
        {
            var testObject = new object();

            IAttachment attachment = new TestAttachment(testObject);

            attachment.AttachTo(testObject);
        }
    }
}