using System.IO;
using System.Text;
using Machine.Specifications;
using Particular.ServiceInsight.Desktop.ExtensionMethods;

namespace Particular.ServiceInsight.Tests.Streams
{
    [Subject("streams")]
    public class when_working_with_stream_of_strings
    {
        protected static Stream SourceStream;
        protected static string SourceMesage;

        Establish context = () =>
        {
            SourceMesage = "This is a sample message";
            SourceStream = new MemoryStream(Encoding.Default.GetBytes(SourceMesage));
        };
    }

    public class when_converting_from_stream_to_string : when_working_with_stream_of_strings
    {
        protected static string Result;

        Establish context = () => SourceStream.Position = 5;

        Because of = () => Result = SourceStream.GetAsString();

        It should_read_the_stream_as_a_string = () => Result.ShouldEqual(SourceMesage);
        It should_preserve_position_of_the_stream = () => SourceStream.Position.ShouldEqual(5);
    }

    public class when_converting_a_string_to_byte_array : when_working_with_stream_of_strings
    {
        protected static Stream ConvertedStream;

        Because of = () => ConvertedStream = SourceMesage.GetAsStream();

        It should_convert_to_the_same_stream_length = () => ConvertedStream.Length.ShouldEqual(SourceStream.Length);
    }
}