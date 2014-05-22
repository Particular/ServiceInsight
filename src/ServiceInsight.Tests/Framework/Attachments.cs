namespace Particular.ServiceInsight.Tests.Framework
{
    using System;
    using Desktop.Framework.Attributes;
    using NUnit.Framework;

    [TestFixture]
    public class Attachments
    {
        private class TestAttachment : Attachment<object>
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
        public void attachment_sets_instance_before_onattach()
        {
            var testObject = new Object();

            IAttachment attachment = new TestAttachment(testObject);

            attachment.AttachTo(testObject);
        }
    }
}