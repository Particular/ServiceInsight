namespace Particular.ServiceInsight.Desktop.CodeParser
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Documents;
    using System.Windows.Media;

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

        public override Inline ToInline(CodeLexem codeLexem)
        {
            switch (codeLexem.Type)
            {
                case LexemType.Error:
                    return CreateRun(codeLexem.Text, Colors.LightGray);
                case LexemType.Symbol:
                    return CreateRun(codeLexem.Text, Colors.Blue);
                case LexemType.Object:
                    return CreateRun(codeLexem.Text, Colors.Brown);
                case LexemType.Property:
                    return CreateRun(codeLexem.Text, Colors.Red);
                case LexemType.Value:
                    return CreateRun(codeLexem.Text, Colors.Blue);
                case LexemType.Space:
                    return CreateRun(codeLexem.Text, Colors.Black);
                case LexemType.LineBreak:
                    return new LineBreak();
                case LexemType.Complex:
                    return CreateRun(codeLexem.Text, Colors.LightGray);
                case LexemType.Comment:
                    return CreateRun(codeLexem.Text, Colors.Green);
                case LexemType.PlainText:
                    return CreateRun(codeLexem.Text, Colors.Black);
                case LexemType.String:
                    return CreateRun(codeLexem.Text, Colors.Brown);
                case LexemType.KeyWord:
                    return CreateRun(codeLexem.Text, Colors.Blue);
                case LexemType.Quotes:
                    return CreateRun(codeLexem.Text, Colors.Blue);
            }

            throw new NotImplementedException(string.Format("Lexem type {0} has no specific colors.", codeLexem.Type));

        }
    }
}