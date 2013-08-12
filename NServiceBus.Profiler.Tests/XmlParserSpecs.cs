using System.Collections.Generic;
using System.Windows.Documents;
using Machine.Specifications;
using System.Linq;
using Particular.ServiceInsight.Desktop.CodeParser;

namespace NServiceBus.Profiler.Tests.Parsers
{
    [Subject("xml code parser")]
    public abstract class with_a_xml_parser
    {
        protected static XmlParser Parser;
        protected static IList<CodeLexem> Lexemes;

        Establish context = () =>
        {
            Parser = new XmlParser();
        };
    }

    public class when_parsing_simple_xml_strings : with_a_xml_parser
    {
        protected static string SimpleXml = "<xml version=\"1.0\" encoding=\"utf-8\"><packages>test package</packages>";

        Because of = () => Lexemes = Parser.Parse(SimpleXml);

        It should_parse_opening_bracket = () =>
        {
            Lexemes[0].Type.ShouldEqual(LexemType.Symbol);
            Lexemes[0].Text.ShouldEqual("<");
        };

        It should_parse_xml_tag = () =>
        {
            Lexemes[1].Type.ShouldEqual(LexemType.Object);
            Lexemes[1].Text.ShouldEqual("xml");
        };

        It should_parse_all_spaces = () => Lexemes.Count(lx => lx.Type == LexemType.Space).ShouldEqual(3);
        It should_parse_all_properties = () => Lexemes.Count(lx => lx.Type == LexemType.Property).ShouldEqual(2);
        It should_parse_all_symbols = () => Lexemes.Count(lx => lx.Type == LexemType.Symbol).ShouldEqual(8); // Followings are symbols: <>=
        It should_parse_all_values = () => Lexemes.Count(lx => lx.Type == LexemType.Value).ShouldEqual(2); // Followings are values: 1.0, utf-8
        It should_parse_all_plain_texts = () => Lexemes.Count(lx => lx.Type == LexemType.PlainText).ShouldEqual(2); // Followings are values: test, package
        It should_parse_all_quotes = () => Lexemes.Count(lx => lx.Type == LexemType.Quotes).ShouldEqual(4); // Followings are quotes: " or '
        It should_parse_all_objects = () => Lexemes.Count(lx => lx.Type == LexemType.Object).ShouldEqual(3); // Followings are objects: xml and packages (2 of them)
    }

    public class when_parsing_more_complex_xml_content : with_a_xml_parser
    {
        protected static string ComplexXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"+
                                             "<data xmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\"" +
                                             "<!--  A SAMPLE set of slides  -->"+
                                             "<slideshow title=\"Sample Slide Show\" "+
                                                         "date=\"Date of publication\" "+
                                                         "author=\"Yours Truly\">"+
                                                  "<!-- TITLE SLIDE -->"+
                                                  "<slide type=\"all\">"+
                                                    "<title>Wake up to WonderWidgets!</title>"+
                                                  "</slide>"+
                                                  "<!-- OVERVIEW -->"+
                                                  "<d:slide type=\"all\">"+
                                                    "<title>Overview</title>"+
                                                    "<item>Why WonderWidgets are great</item>"+
                                                    "<item/>"+
                                                    "<item>Who buys WonderWidgets</item>"+
                                                  "</d:slide>" +
                                             "</slideshow>" +
                                             "</data>";

        Because of = () => Lexemes = Parser.Parse(ComplexXml);

        It shoud_parse_all_content = () => Lexemes.Count.ShouldEqual(136);
        It shoud_parse_all_comments = () => Lexemes.Count(lx => lx.Type == LexemType.Comment).ShouldEqual(6);
    }

    public class when_colorizing_xml_string : with_a_xml_parser
    {
        protected static string ComplexXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"+
                                             "<data xmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\"" +
                                             "<!--  A SAMPLE set of slides  -->"+
                                             "<slideshow title=\"Sample Slide Show\" "+
                                                         "date=\"Date of publication\" "+
                                                         "author=\"Yours Truly\">"+
                                                  "<!-- TITLE SLIDE -->"+
                                                  "<slide type=\"all\">"+
                                                    "<title>Wake up to WonderWidgets!</title>"+
                                                  "</slide>"+
                                                  "<!-- OVERVIEW -->"+
                                                  "<d:slide type=\"all\">"+
                                                    "<title>Overview</title>"+
                                                    "<item>Why WonderWidgets are great</item>"+
                                                    "<item/>"+
                                                    "<item>Who buys WonderWidgets</item>"+
                                                  "</d:slide>" +
                                             "</slideshow>" +
                                             "</data>";
        protected static List<CodeLexem> Lexems;
        protected static Paragraph paragraph;

        Because of = () =>
        {
            paragraph = new Paragraph();
            new CodeBlockPresenter(CodeLanguage.Xml).FillInlines(ComplexXml, paragraph.Inlines);
        };

        It should_color_objects_with_red = () => paragraph.Inlines.Count.ShouldEqual(136);
    }

    [Subject("xml code parser")]
    public class when_having_empty_lexem
    {
        protected static CodeLexem lexem;

        It should_just_have_a_end_of_line = () => new CodeLexem("").Parse(CodeLanguage.Plain).Count.ShouldEqual(1);
    }
}