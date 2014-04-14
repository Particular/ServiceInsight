using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using NServiceBus.Profiler.Desktop.CodeParser;
using NUnit.Framework;
using Shouldly;

namespace NServiceBus.Profiler.Tests
{
    [TestFixture]
    public class XmlParserTests
    {
        private XmlParser Parser;
        private IList<CodeLexem> Lexemes;

        const string SimpleXml = "<xml version=\"1.0\" encoding=\"utf-8\"><packages>test package</packages>";

        const string ComplexXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                             "<data xmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\"" +
                                             "<!--  A SAMPLE set of slides  -->" +
                                             "<slideshow title=\"Sample Slide Show\" " +
                                                         "date=\"Date of publication\" " +
                                                         "author=\"Yours Truly\">" +
                                                  "<!-- TITLE SLIDE -->" +
                                                  "<slide type=\"all\">" +
                                                    "<title>Wake up to WonderWidgets!</title>" +
                                                  "</slide>" +
                                                  "<!-- OVERVIEW -->" +
                                                  "<d:slide type=\"all\">" +
                                                    "<title>Overview</title>" +
                                                    "<item>Why WonderWidgets are great</item>" +
                                                    "<item/>" +
                                                    "<item>Who buys WonderWidgets</item>" +
                                                  "</d:slide>" +
                                             "</slideshow>" +
                                             "</data>";

        [SetUp]
        public void TestInitialize()
        {
            Parser = new XmlParser();
        }

        [Test]
        public void should_parse_objects_symbols_and_properties_when_parsing_simple_xml_strings()
        {
            Lexemes = Parser.Parse(SimpleXml);

            Lexemes[0].Type.ShouldBe(LexemType.Symbol);
            Lexemes[0].Text.ShouldBe("<");

            Lexemes[1].Type.ShouldBe(LexemType.Object);
            Lexemes[1].Text.ShouldBe("xml");

            Lexemes.Count(lx => lx.Type == LexemType.Space).ShouldBe(3);
            Lexemes.Count(lx => lx.Type == LexemType.Property).ShouldBe(2);
            Lexemes.Count(lx => lx.Type == LexemType.Symbol).ShouldBe(8); // Followings are symbols: <>=
            Lexemes.Count(lx => lx.Type == LexemType.Value).ShouldBe(2); // Followings are values: 1.0, utf-8
            Lexemes.Count(lx => lx.Type == LexemType.PlainText).ShouldBe(2); // Followings are values: test, package
            Lexemes.Count(lx => lx.Type == LexemType.Quotes).ShouldBe(4); // Followings are quotes: " or '
            Lexemes.Count(lx => lx.Type == LexemType.Object).ShouldBe(3); // Followings are objects: xml and packages (2 of them)
        }

        [Test]
        public void should_parse_objects_symbols_and_properties_when_parsing_more_complex_xml_content()
        {
            Lexemes = Parser.Parse(ComplexXml);

            Lexemes.Count.ShouldBe(136);
            Lexemes.Count(lx => lx.Type == LexemType.Comment).ShouldBe(6);
        }

        [Test]
        public void should_color_objects_with_red_when_colorizing_xml_string()
        {
            var paragraph = new Paragraph();

            new CodeBlockPresenter(CodeLanguage.Xml).FillInlines(ComplexXml, paragraph.Inlines);

            paragraph.Inlines.Count.ShouldBe(136);
        }

        [Test]
        public void should_just_have_a_end_of_line_when_having_empty_lexem()
        {
            new CodeLexem("").Parse(CodeLanguage.Plain).Count.ShouldBe(1);
        }
    }
}