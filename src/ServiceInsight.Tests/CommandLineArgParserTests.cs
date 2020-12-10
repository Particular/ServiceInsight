namespace ServiceInsight.Tests
{
    using NSubstitute;
    using NUnit.Framework;
    using ServiceInsight.Models;
    using ServiceInsight.Startup;
    using Shouldly;

    [TestFixture]
    public class CommandLineArgParserTests
    {
        const string AppPath = @"C:\Program Files\Particular\ServiceInsight\ServiceInsight.exe";
        const string SchemaPrefix = CommandLineOptions.ApplicationScheme;

        EnvironmentWrapper environment;

        [SetUp]
        public void Initialize()
        {
            environment = Substitute.For<EnvironmentWrapper>();
        }

        [Test]
        public void Strips_application_path_from_the_command_line_args()
        {
            const string Uri = "localhost:12345";
            const string ExpectedUri = "http://localhost:12345/";

            var invocationParameters = string.Format("{0}{1}", SchemaPrefix, Uri);

            environment.GetCommandLineArgs().Returns(new[] { AppPath, invocationParameters });

            var sut = CreateSut();

            sut.ParsedOptions.EndpointUri.ToString().ShouldBe(ExpectedUri);
        }

        [Test]
        public void Can_switch_to_secured_connection()
        {
            const string Options = "SecuredConnection=True";
            const string Uri = "localhost:12345";
            var invocationParameters = $"{SchemaPrefix}{Uri}?{Options}";

            environment.GetCommandLineArgs().Returns(new[] { AppPath, invocationParameters });

            var sut = CreateSut();

            sut.ParsedOptions.SecuredConnection.ShouldBe(true);
            sut.ParsedOptions.EndpointUri.Scheme.ShouldBe("https", StringCompareShould.IgnoreCase);
        }

        [Test]
        [TestCase("localhost")]
        [TestCase("127.0.0.1")]
        [TestCase("192.168.0.100")]
        [TestCase("servicecontrol.myserver.com")]
        [TestCase("servicecontrol.myserver.com/")]
        [TestCase("servicecontrol.myserver.com/api")]
        [TestCase("servicecontrol.myserver.com:33333/api")]
        [TestCase("anyknownserver:33333/api")]
        [TestCase("anyknownserver/api")]
        public void Can_parse_valid_uri_from_command_line_args(string validUri)
        {
            var invocationParameters = string.Format("{0}{1}", SchemaPrefix, validUri);

            environment.GetCommandLineArgs().Returns(new[] { AppPath, invocationParameters });

            var sut = CreateSut();

            sut.ParsedOptions.EndpointUri.ShouldNotBe(null);
            sut.ParsedOptions.EndpointUri.ToString().ShouldContain(validUri);
        }

        [Test]
        [TestCase(".")]
        [TestCase("./api")]
        [TestCase("::1")]
        [TestCase("::1/api")]
        [TestCase("192.168.1/api")]
        [TestCase("tcp://192.168.0.1/api")]
        [TestCase("ucp://localhost/api")]
        public void Should_not_parse_invalid_uri_from_command_line_args(string invalidUri)
        {
            environment.GetCommandLineArgs().Returns(new[] { AppPath, invalidUri });

            var sut = CreateSut();

            sut.ParsedOptions.EndpointUri.ShouldBe(null);
        }

        [Test]
        public void Can_parse_endpoint_name_from_command_line_arg()
        {
            const string EndpointName = "VideoStore.Sales";
            const string Uri = "localhost:12345";
            var invocationParameters = string.Format("{0}{1}?EndpointName={2}", SchemaPrefix, Uri, EndpointName);

            environment.GetCommandLineArgs().Returns(new[] { AppPath, invocationParameters });

            var sut = CreateSut();

            sut.ParsedOptions.EndpointName.ShouldBe(EndpointName);
        }

        [Test]
        public void Can_parse_and_decode_search_query_from_command_line_arg()
        {
            const string SearchQuery = "Sample%20Search%20Criteria";
            const string Uri = "localhost:12345";
            var invocationParameters = string.Format("{0}{1}?Search={2}", SchemaPrefix, Uri, SearchQuery);

            environment.GetCommandLineArgs().Returns(new[] { AppPath, invocationParameters });

            var sut = CreateSut();

            sut.ParsedOptions.SearchQuery.ShouldBe("Sample Search Criteria");
        }

        [Test]
        public void Can_parse_auto_refresh_rate_from_command_line_arg()
        {
            const string AutoRefreshRate = "100";
            const string Uri = "localhost:12345";
            var invocationParameters = string.Format("{0}{1}?AutoRefresh={2}", SchemaPrefix, Uri, AutoRefreshRate);

            environment.GetCommandLineArgs().Returns(new[] { AppPath, invocationParameters });

            var sut = CreateSut();

            sut.ParsedOptions.AutoRefreshRate.ShouldBe(100);
        }

        [Test]
        public void Passing_wrong_key_will_throw()
        {
            const string Uri = "localhost:12345";
            const string UnsupportedKey = "UnsupportedKey";
            var invocationParameters = string.Format("{0}{1}?{2}=value", SchemaPrefix, Uri, UnsupportedKey);

            environment.GetCommandLineArgs().Returns(new[] { AppPath, invocationParameters });

            var sut = CreateSut();

            sut.HasUnsupportedKeys.ShouldBe(true);
        }

        CommandLineArgParser CreateSut()
        {
            var parser = new CommandLineArgParser(environment);
            return parser;
        }
    }
}