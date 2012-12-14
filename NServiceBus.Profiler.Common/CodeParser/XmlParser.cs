using System;
using System.Collections.Generic;

namespace NServiceBus.Profiler.Common.CodeParser
{
    public class XmlParser : BaseParser
    {
        protected static readonly char[] XmlEndOfTerm = new[] { ' ', '\t', '\n', '=', '/', '>', '<', '"', '{', '}', ',' };
        protected static readonly char[] XmlSymbol = new[] { '=', '/', '>', '"', '{', '}', ',' };
        protected static readonly char[] XmlQuotes = new[] { '"'  };
        protected static readonly char XmlNamespaceDelimeter = ':';

        protected bool IsInsideBlock;

        protected override List<CodeLexem> Parse(SourcePart text)
        {
            var list = new List<CodeLexem>();
            while (text.Length > 0)
            {
                if (TryExtract(list, ref text, "<!--", LexemType.Comment))
                    TryExtractTo(list, ref text, "-->", LexemType.Comment);

                if (text.StartsWith("<", StringComparison.Ordinal))
                    IsInsideBlock = false;

                if (TryExtract(list, ref text, "\"{}", LexemType.Value))
                    TryExtractTo(list, ref text, "\"", LexemType.Value);

                if (TryExtract(list, ref text, "</", LexemType.Symbol) ||
                    TryExtract(list, ref text, "<", LexemType.Symbol) ||
                    TryExtract(list, ref text, "{", LexemType.Symbol))
                {
                    ParseXmlKeyWord(list, ref text, LexemType.Object);
                }

                if (TryExtract(list, ref text, "\"", LexemType.Quotes))
                {
                    ParseValue(list, ref text);
                }

                ParseXmlKeyWord(list, ref text, IsInsideBlock ? LexemType.PlainText : LexemType.Property);
                TryExtract(list, ref text, "\"", LexemType.Quotes);
                TryExtract(list, ref text, "}", LexemType.Symbol);
                
                if (text.StartsWith(">", StringComparison.Ordinal))
                    IsInsideBlock = true;

                ParseSymbol(list, ref text);
                TrySpace(list, ref text);
                TryExtract(list, ref text, "\n", LexemType.LineBreak);
            }
            return list;
        }

        protected void ParseValue(List<CodeLexem> res, ref SourcePart text)
        {
            const string lex = "\"";
            var index = text.IndexOf(lex);

            if (index < 0)
                return;
            
            LineBreaks(res, ref text, index + lex.Length - 1, LexemType.Value);
        }

        private void ParseSymbol(ICollection<CodeLexem> res, ref SourcePart text)
        {
            int index = text.IndexOfAny(XmlSymbol);
            if (index != 0)
                return;
            
            res.Add(new CodeLexem(LexemType.Symbol, text.Substring(0, 1)));
            text = text.Substring(1);
        }

        private void ParseXmlKeyWord(ICollection<CodeLexem> res, ref SourcePart text, LexemType type)
        {
            var index = text.IndexOfAny(XmlEndOfTerm);
            if (index <= 0)
                return;
            
            var delimiterIndex = text.IndexOf(XmlNamespaceDelimeter);
            if (delimiterIndex > 0 && delimiterIndex < index)
            {
                res.Add(new CodeLexem(type, CutString(ref text, delimiterIndex)));
                res.Add(new CodeLexem(LexemType.Symbol, CutString(ref text, 1)));
                res.Add(new CodeLexem(type, CutString(ref text, index - delimiterIndex - 1)));
                return;
            }
            res.Add(new CodeLexem(type, CutString(ref text, index)));
        }
    }
}