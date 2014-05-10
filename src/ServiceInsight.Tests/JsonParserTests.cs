namespace Particular.ServiceInsight.Tests
{
    using Desktop.CodeParser;
    using NUnit.Framework;
    using System.Linq;
    using Shouldly;

    [TestFixture]
    public class JsonParserTests
    {
        [Test]
        public void should_parse_properties_objects_and_symbols()
        {
            const string TestMessage = "﻿[{\"$type\":\"NSB.Messages.CRM.RegisterCustomer, NSB.Messages\",\"Name\":\"Hadi\",\"Password\":\"123456\",\"EmailAddress\":\"h.eskandari@gmail.com\",\"RegistrationDate\":\"2013-01-28T03:24:05.0546437Z\"}]";

            var lexemes = new CodeLexem(TestMessage).Parse(CodeLanguage.Json);

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

            lexemes.Count(lx => lx.Type == LexemType.Property).ShouldBe(11);
            lexemes.Count(lx => lx.Type == LexemType.Value).ShouldBe(8);
            lexemes.Count(lx => lx.Type == LexemType.LineBreak).ShouldBe(11);
        }
    }
}