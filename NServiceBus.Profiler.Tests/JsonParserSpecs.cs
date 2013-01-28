using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using NServiceBus.Profiler.Common.CodeParser;

namespace NServiceBus.Profiler.Tests.Parsers
{
    [Subject("json code parser")]
    public abstract class with_a_json_parser
    {
        protected static JsonParser Parser;
        protected static IList<CodeLexem> Lexemes;

        Establish context = () =>
        {
            Parser = new JsonParser();
        };
    }

    public class when_parsing_simple_json_string : with_a_json_parser
    {
        protected static string TestMessage = "﻿[{\"$type\":\"NSB.Messages.CRM.RegisterCustomer, NSB.Messages\",\"Name\":\"Hadi\",\"Password\":\"123456\",\"EmailAddress\":\"h.eskandari@gmail.com\",\"RegistrationDate\":\"2013-01-28T03:24:05.0546437Z\"}]";

        Because of = () => Lexemes = Parser.Parse(TestMessage);

        It should_parse_all_properties = () => Lexemes.Count(lx => lx.Type == LexemType.Property).ShouldEqual(5);
        It should_parse_all_property_values = () => Lexemes.Count(lx => lx.Type == LexemType.Value).ShouldEqual(5);
        It should_parse_all_objects = () => Lexemes.Count(lx => lx.Type == LexemType.Quotes).ShouldEqual(20);
        It should_parse_all_symbols = () => Lexemes.Count(lx => lx.Type == LexemType.Symbol).ShouldEqual(13);
    }

    public class when_parsing_json_object_graph : with_a_json_parser
    {
        protected static string JsonGraph = @"{""menu"": {
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

        Because of = () => Lexemes = Parser.Parse(JsonGraph);

        It should_parse_all_properties = () => Lexemes.Count(lx => lx.Type == LexemType.Property).ShouldEqual(11);
        It should_parse_all_property_values = () => Lexemes.Count(lx => lx.Type == LexemType.Value).ShouldEqual(8);
        It should_parse_all_line_breaks = () => Lexemes.Count(lx => lx.Type == LexemType.LineBreak).ShouldEqual(11);
    }
}