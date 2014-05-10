namespace Particular.ServiceInsight.Tests
{
    using System.Text;
    using Desktop.Core.MessageDecoders;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class StringDecoderTests
    {
        private byte[] stringContent;
        private IContentDecoder decoder;

        [SetUp]
        public void TestInitialize()
        {
            stringContent = Encoding.UTF8.GetBytes("This is a string content");
            decoder = new StringContentDecoder();
        }

        [Test]
        public void should_be_able_to_decode_conent_to_string()
        {
            decoder.Decode(stringContent).IsParsed.ShouldBe(true);
        }

        [Test]
        public void should_decode_conent_to_string()
        {
            decoder.Decode(stringContent).Value.ShouldBe("This is a string content");
        }

        [Test]
        public void should_return_empty_string_if_there_is_no_content()
        {
            decoder.Decode(new byte[0]).Value.ShouldBe("");
        }

        [Test]
        public void should_return_empty_string_if_content_is_null()
        {
            decoder.Decode(null).IsParsed.ShouldBe(false);
        }
    }
}