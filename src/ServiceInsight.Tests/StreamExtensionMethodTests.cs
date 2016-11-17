namespace ServiceInsight.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using NUnit.Framework;
    using ServiceInsight.ExtensionMethods;
    using Shouldly;

    [TestFixture]
    public sealed class StreamExtensionMethodTests : IDisposable
    {
        const string SourceMesage = "This is a sample message";
        MemoryStream sourceStream;

        public StreamExtensionMethodTests()
        {
            sourceStream = new MemoryStream(Encoding.Default.GetBytes(SourceMesage));
        }

        public void Dispose()
        {
            sourceStream.Dispose();
        }

        [Test]
        public void Converting_from_stream_to_string()
        {
            var result = sourceStream.GetAsString();

            result.ShouldBe(SourceMesage);
        }

        [Test]
        public void Should_preserve_positions_when_converting_to_string()
        {
            sourceStream.Position = 5;

            var result = sourceStream.GetAsString();

            result.ShouldBe(SourceMesage);
            sourceStream.Position.ShouldBe(5);
        }

        [Test]
        public void Should_convert_to_the_same_stream_length()
        {
            var convertedStream = SourceMesage.GetAsStream();

            convertedStream.Length.ShouldBe(sourceStream.Length);
        }
    }
}