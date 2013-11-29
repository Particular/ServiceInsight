using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace NServiceBus.Profiler.Desktop.CodeParser
{
    public class CodeLexem
    {
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

        public Inline ToInline(CodeLanguage lang)
        {
            switch (lang)
            {
                case CodeLanguage.Xml:
                    return new XmlParser().ToInline(this);
                case CodeLanguage.Json:
                    return new JsonParser().ToInline(this);
                case CodeLanguage.Plain:
                    return new BaseParser().ToInline(this);
                default:
                    throw new NotImplementedException(string.Format("Conversion from {0} language is not supported.", lang));
            }
        }

        public override string ToString()
        {
            return string.Format("Text: {0}  Type: {1}", Text, Type);
        }
    }
}