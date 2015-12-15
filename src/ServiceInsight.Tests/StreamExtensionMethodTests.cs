namespace ServiceInsight.Tests
{
    using System.IO;
    using System.Text;
    using ServiceInsight.ExtensionMethods;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class StreamExtensionMethodTests
    {
        const string SourceMesage = "This is a sample message";
        MemoryStream SourceStream;

        [SetUp]
        public void TestInitialize()
        {
            SourceStream = new MemoryStream(Encoding.Default.GetBytes(SourceMesage));
        }

        [Test]
        public void converting_from_stream_to_string()
        {
            var result = SourceStream.GetAsString();

            result.ShouldBe(SourceMesage);
        }

        [Test]
        public void should_preserve_positions_when_converting_to_string()
        {
            SourceStream.Position = 5;

            var result = SourceStream.GetAsString();

            result.ShouldBe(SourceMesage);
            SourceStream.Position.ShouldBe(5);
        }

        [Test]
        public void should_convert_to_the_same_stream_length()
        {
            var convertedStream = SourceMesage.GetAsStream();

            convertedStream.Length.ShouldBe(SourceStream.Length);
        }
    }
}