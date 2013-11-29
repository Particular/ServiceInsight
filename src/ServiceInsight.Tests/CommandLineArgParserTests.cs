using System;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.Desktop.Startup;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace NServiceBus.Profiler.Tests
{
    [TestFixture]
    public class CommandLineArgParserTests
    {
        private const string AppPath = @"C:\Program Files\Particular\ServiceInsight\ServiceInsight.exe";
        private const string SchemaPrefix = CommandLineOptions.ApplicationScheme;

        private IEnvironment _environment;

        [SetUp]
        public void Initialize()
        {
            _environment = Substitute.For<IEnvironment>();            
        }

        [Test]
        public void strips_application_path_from_the_command_line_args()
        {
            const string Uri = "http://localhost:12345/";
            var invocationParameters = string.Format("{0}{1}", SchemaPrefix, Uri);
            
            _environment.GetCommandLineArgs().Returns(new[] { AppPath, invocationParameters });

            var sut = CreateSut();

            sut.ParsedOptions.EndpointUri.ToString().ShouldBe(Uri);
        }

        [Test]
        public void can_parse_endpoint_name_from_command_line_arg()
        {
            const string EndpointName = "VideoStore.Sales";
            const string Uri = "http://localhost:12345/";
            var invocationParameters = string.Format("{0}{1}&EndpointName={2}", SchemaPrefix, Uri, EndpointName);

            _environment.GetCommandLineArgs().Returns(new[] { AppPath, invocationParameters });

            var sut = CreateSut();

            sut.ParsedOptions.EndpointName.ShouldBe(EndpointName);
        }

        [Test]
        public void can_parse_and_decode_search_query_from_command_line_arg()
        {
            const string SearchQuery = "Sample%20Search%20Criteria";
            const string Uri = "http://localhost:12345/";
            var invocationParameters = string.Format("{0}{1}&Search={2}", SchemaPrefix, Uri, SearchQuery);

            _environment.GetCommandLineArgs().Returns(new[] { AppPath, invocationParameters });

            var sut = CreateSut();

            sut.ParsedOptions.SearchQuery.ShouldBe("Sample Search Criteria");
        }

        [Test]
        public void can_parse_auto_refresh_rate_from_command_line_arg()
        {
            const string AutoRefreshRate = "100";
            const string Uri = "http://localhost:12345/";
            var invocationParameters = string.Format("{0}{1}&AutoRefresh={2}", SchemaPrefix, Uri, AutoRefreshRate);

            _environment.GetCommandLineArgs().Returns(new[] { AppPath, invocationParameters });

            var sut = CreateSut();

            sut.ParsedOptions.AutoRefreshRate.ShouldBe(100);
        }

        [Test]
        [TestCase("http://thisiswrong")]
        [TestCase("somerandomname")]
        [TestCase("ftp://localhost")]
        public void leaves_service_uri_empty_if_passed_in_argument_is_not_valid(string wrongUri)
        {
            var invocationParameters = string.Format("{0}{1}", SchemaPrefix, wrongUri);

            _environment.GetCommandLineArgs().Returns(new[] { AppPath, invocationParameters });

            var sut = CreateSut();

            sut.ParsedOptions.EndpointUri.ShouldBe(null);
        }

        [Test]
        public void passing_wrong_key_will_throw()
        {
            const string Uri = "http://localhost:12345/";
            const string UnsupportedKey = "UnsupportedKey";
            var invocationParameters = string.Format("{0}{1}&{2}=value", SchemaPrefix, Uri, UnsupportedKey);

            _environment.GetCommandLineArgs().Returns(new[] { AppPath, invocationParameters });

            var exception = Should.Throw<NotSupportedException>(() => CreateSut());

            exception.Message.ShouldContain(UnsupportedKey);
        }

        private ICommandLineArgParser CreateSut()
        {
            var parser = new CommandLineArgParser(_environment);
            parser.Start();
            return parser;
        }
    }
}