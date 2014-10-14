namespace Particular.ServiceInsight.Desktop.CodeParser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Documents;
    using System.Windows.Media;
    using Newtonsoft.Json;

    public class JsonParser : BaseParser
    {
        protected override List<CodeLexem> Parse(SourcePart text)
        {
            var list = new List<CodeLexem>();

            TryExtract(ref text, ByteOrderMark);

            var s = text.Substring(0, text.Length);

            using (var sreader = new StringReader(s))
            using (var reader = new JsonTextReader(sreader))
            {
                reader.DateParseHandling = DateParseHandling.None;

                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.StartArray: Extract(list, ref text, "[", LexemType.Symbol); break;
                        case JsonToken.EndArray: Extract(list, ref text, "]", LexemType.Symbol); break;

                        case JsonToken.StartObject: Extract(list, ref text, "{", LexemType.Symbol); break;
                        case JsonToken.EndObject: Extract(list, ref text, "}", LexemType.Symbol); break;

                        case JsonToken.PropertyName: Extract(list, ref text, reader.Value.ToString(), LexemType.Property); break;

                        case JsonToken.Raw:
                        case JsonToken.Integer:
                        case JsonToken.Float:
                        case JsonToken.String:
                        case JsonToken.Boolean:
                        case JsonToken.Date:
                        case JsonToken.Bytes:
                            Extract(list, ref text, reader.Value.ToString(), LexemType.Value);
                            break;

                        case JsonToken.Null:
                            Extract(list, ref text, "null", LexemType.Value);
                            break;

                        case JsonToken.Comment: Extract(list, ref text, reader.Value.ToString(), LexemType.Comment); break;

                        default: list.Add(new CodeLexem(LexemType.Error, reader.TokenType.ToString())); break;
                    }
                }
            }

            while (text.Length > 0)
            {
                var length = text.Length;

                ExtractUnparsedItems(list, ref text);

                if (length == text.Length)
                    break;
            }

            return list;
        }

        void Extract(List<CodeLexem> res, ref SourcePart text, string lex, LexemType type)
        {
            while (!TryExtract(res, ref text, lex, type))
            {
                var length = text.Length;

                ExtractUnparsedItems(res, ref text);

                if (length == text.Length)
                    break;
            }
        }

        void ExtractUnparsedItems(List<CodeLexem> res, ref SourcePart text)
        {
            TrySpace(res, ref text);
            TryExtract(res, ref text, "\r\n", LexemType.LineBreak);
            TryExtract(res, ref text, "\n", LexemType.LineBreak);
            TryExtract(res, ref text, "\"", LexemType.Quotes);
            TryExtract(res, ref text, ",", LexemType.Symbol);
            TryExtract(res, ref text, ":", LexemType.Symbol);
        }

        public override Inline ToInline(CodeLexem codeLexem, Brush brush)
        {
            if (brush != null) return base.ToInline(codeLexem, brush);

            switch (codeLexem.Type)
            {
                case LexemType.Symbol:
                case LexemType.Object:
                    return CreateRun(codeLexem.Text, Colors.LightGray);

                case LexemType.Property:
                    return CreateRun(codeLexem.Text, Colors.Blue);

                case LexemType.Value:
                case LexemType.Space:
                case LexemType.PlainText:
                case LexemType.String:
                    return CreateRun(codeLexem.Text, Colors.Black);

                case LexemType.LineBreak:
                    return new LineBreak();

                case LexemType.Complex:
                case LexemType.Quotes:
                    return CreateRun(codeLexem.Text, Colors.Brown);
            }

            throw new NotImplementedException(string.Format("Lexem type {0} has no specific colors.", codeLexem.Type));
        }
    }
}