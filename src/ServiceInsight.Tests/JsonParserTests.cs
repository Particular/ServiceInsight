namespace Particular.ServiceInsight.Tests
{
    using System.Linq;
    using Desktop.CodeParser;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class JsonParserTests
    {
        [Test]
        public void should_handle_null_value_without_quotes()
        {
            const string TestMessage = "{\"Id\":\"outer\",\"SubMessages\":[{\"Id\":\"inner1\",\"SubMessages\":null},{\"Id\":\"inner2\",\"SubMessages\":null}]}";

            var lexemes = new CodeLexem(TestMessage).Parse(CodeLanguage.Json);
            var values = lexemes.Where(lx => lx.Type == LexemType.Value).ToList();

            lexemes.Count.ShouldBe(48);
            values.Count.ShouldBe(5);
            values[0].Text.ShouldBe("outer");
            values[1].Text.ShouldBe("inner1");
            values[2].Text.ShouldBe("null");
            values[3].Text.ShouldBe("inner2");
            values[4].Text.ShouldBe("null");
        }

        [Test]
        public void should_parse_values_from_json()
        {
            const string TestMessage = "[\n  \"shiny\",\n  \"day\",\n  \"need\",\n  null]";

            var lexemes = new CodeLexem(TestMessage).Parse(CodeLanguage.Json);
            var values = lexemes.Where(lx => lx.Type == LexemType.Value).ToList();

            lexemes.Count.ShouldBe(24);
            values.Count.ShouldBe(4);
            values[0].Text.ShouldBe("shiny");
            values[1].Text.ShouldBe("day");
            values[2].Text.ShouldBe("need");
            values[3].Text.ShouldBe("null");
        }

        [Test]
        public void should_parse_properties_objects_and_symbols()
        {
            const string TestMessage = "[{\"$type\":\"NSB.Messages.CRM.RegisterCustomer, NSB.Messages\",\"Name\":\"Hadi\",\"Password\":\"123456\",\"EmailAddress\":\"h.eskandari@gmail.com\",\"RegistrationDate\":\"2013-01-28T03:24:05.0546437Z\"}]";

            var lexemes = new CodeLexem(TestMessage).Parse(CodeLanguage.Json);

            lexemes.Count().ShouldBe(44);
            lexemes.Count(lx => lx.Type == LexemType.Property).ShouldBe(5);
            lexemes.Count(lx => lx.Type == LexemType.Value).ShouldBe(5);
            lexemes.Count(lx => lx.Type == LexemType.Quotes).ShouldBe(20);
            lexemes.Count(lx => lx.Type == LexemType.Symbol).ShouldBe(13);
        }

        [Test]
        public void should_parse_complex_json_graphs()
        {
            const string JsonGraph = @"{""menu"": {
  ""id"": ""file"",
  ""value"": ""File"",
  ""popup"": {
    ""menuitem"": [
      {""value"": ""New"", ""onclick"": ""CreateNewDoc()""},
      {""value"": ""Open"", ""onclick"": ""OpenDoc()""},
      {""value"": ""Close"", ""onclick"": ""CloseDoc()""}
    ]
  }
}}";

            var lexemes = new CodeLexem(JsonGraph).Parse(CodeLanguage.Json);

            lexemes.Count().ShouldBe(123);
            lexemes.Count(lx => lx.Type == LexemType.Property).ShouldBe(11);
            lexemes.Count(lx => lx.Type == LexemType.Value).ShouldBe(8);
            lexemes.Count(lx => lx.Type == LexemType.LineBreak).ShouldBe(11);
        }
    }
}