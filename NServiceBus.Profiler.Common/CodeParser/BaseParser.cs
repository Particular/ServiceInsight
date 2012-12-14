using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceBus.Profiler.Common.CodeParser
{
    public class BaseParser
    {
        private readonly char[] _spaceChars = new[] {' ', '\t'};
        protected char previousSimbol;

        protected string CutString(ref SourcePart text, int count)
        {
            if (count == 0)
                return string.Empty;
            
            previousSimbol = text[count - 1];
            var result = text.Substring(0, count);
            text = text.Substring(count);
            return result;
        }

        protected void TrySpace(List<CodeLexem> res, ref SourcePart text)
        {
            var stringBuilder = new StringBuilder();
            while (_spaceChars.Contains(text[0]))
            {
                stringBuilder.Append(CutString(ref text, 1));
            }

            if (stringBuilder.Length > 0)
            {
                res.Add(new CodeLexem(LexemType.Space, stringBuilder.ToString()));
            }
        }

        protected bool TryExtract(List<CodeLexem> res, ref SourcePart text, string lex, LexemType type)
        {
            if (text.StartsWith(lex, StringComparison.Ordinal))
            {
                res.Add(new CodeLexem(type, CutString(ref text, lex.Length)));
                return true;
            }

            return false;
        }

        protected void TryExtractTo(List<CodeLexem> res, ref SourcePart text, string lex, LexemType type, string except = null)
        {
            var index = text.IndexOf(lex);
            if (except != null)
            {
                while (index >= 0 && text.Substring(0, index + 1).EndsWith(except))
                {
                    index = text.IndexOf(lex, index + 1);
                }
            }
            
            if (index < 0)
                return;
            
            LineBreaks(res, ref text, index + lex.Length, type);
        }

        protected void LineBreaks(List<CodeLexem> res, ref SourcePart text, int to, LexemType type)
        {
            while (text.Length > 0 && to > 0)
            {
                var index = text.IndexOf("\n");
                if (index >= to)
                {
                    res.Add(new CodeLexem(type, CutString(ref text, to)));
                    return;
                }

                if (index != 0)
                {
                    res.Add(new CodeLexem(type, CutString(ref text, index)));
                }

                res.Add(new CodeLexem(LexemType.LineBreak, CutString(ref text, 1)));
                to -= index + 1;
            }
        }

        public List<CodeLexem> Parse(string text)
        {
            return Parse(new SourcePart(text + "\n", 0));
        }

        protected virtual List<CodeLexem> Parse(SourcePart text)
        {
            var list = new List<CodeLexem>();
            var sourceString = text;
            LineBreaks(list, ref sourceString, sourceString.Length, LexemType.PlainText);
            return list;
        }
    }
}