using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media;

namespace NServiceBus.Profiler.Common.CodeParser
{
    public class CodeLexem
    {
        public CodeLexem()
            : this("")
        {
        }

        public CodeLexem(string text)
            : this(LexemType.Complex, text)
        {
        }

        public CodeLexem(LexemType type, string text)
        {
            Text = text;
            Type = type;
        }

        public LexemType Type { get; set; }

        public string Text { get; set; }

        protected Run CreateRun(string text, Color color)
        {
            return new Run
            {
                Text = text,
                Foreground = new SolidColorBrush(color)
            };
        }

        public List<CodeLexem> Parse(CodeLanguage lang)
        {
            switch (lang)
            {
                case CodeLanguage.Plain:
                    return new BaseParser().Parse(Text);
                
                case CodeLanguage.Xml:
                    return new XmlParser().Parse(Text);

                case CodeLanguage.Json:
                    return new JsonParser().Parse(Text);

                default:
                    throw new NotImplementedException(string.Format("Parser for {0} language is not implemented.", lang));
            }
        }

        public Inline ToInline()
        {
            switch (Type)
            {
                case LexemType.Error:
                    return CreateRun(Text, Colors.LightGray);
                case LexemType.Symbol:
                    return CreateRun(Text, Colors.Blue);
                case LexemType.Object:
                    return CreateRun(Text, Colors.Brown);
                case LexemType.Property:
                    return CreateRun(Text, Colors.Red);
                case LexemType.Value:
                    return CreateRun(Text, Colors.Blue);
                case LexemType.Space:
                    return CreateRun(Text, Colors.Black);
                case LexemType.LineBreak:
                    return new LineBreak();
                case LexemType.Complex:
                    return CreateRun(Text, Colors.LightGray);
                case LexemType.Comment:
                    return CreateRun(Text, Colors.Green);
                case LexemType.PlainText:
                    return CreateRun(Text, Colors.Black);
                case LexemType.String:
                    return CreateRun(Text, Colors.Brown);
                case LexemType.KeyWord:
                    return CreateRun(Text, Colors.Blue);
                case LexemType.Quotes:
                    return CreateRun(Text, Colors.Blue);
            }

            throw new NotImplementedException(string.Format("Lexem type {0} has no specific colors.", Type));
        }

        public override string ToString()
        {
            return string.Format("Text: {0}, Type: {1}", Text, Type);
        }
    }
}