using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceBus.Profiler.Common.CodeParser {


  public class JsonParser : BaseParser {


    protected static readonly char[] JsonSymbol = new[] { ':', '[', ']', ',', '{', '}' };
    protected static readonly char[] JsonQuotes = new[] { '"' };

    protected override List<CodeLexem> Parse(SourcePart text) {
      var list = new List<CodeLexem>();

      while( text.Length > 0 ) {

        // Extract Name
        if( TryExtract(list, ref text, "\"", LexemType.Symbol) ) {
          ParseJsonPropertyName(list, ref text);
          TryExtract(list, ref text, "\":", LexemType.Symbol);

          TrySpace(list, ref text);

          // Extract Value
          TryExtractValue(list, ref text);
        }

        // Parse extras
        ParseSymbol(list, ref text);
        
        TrySpace(list, ref text);

        TryExtract(list, ref text, "\n", LexemType.LineBreak);

      }

      return list;
    }

    private void TryExtractValue(List<CodeLexem> res, ref SourcePart text) {

      if( text[0] == '{' ) {
        res.Add(new CodeLexem(LexemType.Symbol, CutString(ref text, 1)));      
      
      } else if( text[0] == '[' ) {
        res.Add(new CodeLexem(LexemType.Symbol, CutString(ref text, 1)));      
      
      } else if( text[0] == '"' ) {
        res.Add(new CodeLexem(LexemType.Symbol, CutString(ref text, 1)));      
        
        int end = text.IndexOf('"');
        res.Add(new CodeLexem(LexemType.PlainText, CutString(ref text, end)));

        res.Add(new CodeLexem(LexemType.Symbol, CutString(ref text, 1)));      
      
      } else { 
        int end = text.IndexOfAny( new char[] { ',', '}' });

        res.Add(new CodeLexem(LexemType.Value, CutString(ref text, end)));

        res.Add(new CodeLexem(LexemType.Symbol, CutString(ref text, 1)));      
      }

      

    }

    private void ParseSymbol(ICollection<CodeLexem> res, ref SourcePart text) {
      int index = text.IndexOfAny(JsonSymbol);
      if( index != 0 )
        return;

      res.Add(new CodeLexem(LexemType.Symbol, text.Substring(0, 1)));
      text = text.Substring(1);
    }

    private void ParseJsonPropertyName(ICollection<CodeLexem> res, ref SourcePart text) {
      var index = text.IndexOf("\":");
      if( index <= 0 )
        return;

      res.Add(new CodeLexem(LexemType.Object, CutString(ref text, index)));
    }

  }
}
