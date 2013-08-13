using System.Windows.Documents;

namespace Particular.ServiceInsight.Desktop.CodeParser
{
    public class CodeBlockPresenter
    {
        public CodeBlockPresenter()
            : this(CodeLanguage.Plain)
        {
        }

        public CodeBlockPresenter(CodeLanguage language)
        {
            CodeLanguage = language;
        }

        public void FillInlines(string text, InlineCollection collection)
        {
            text = text.Replace("\r", "");
            var codeLexem = new CodeLexem(text);
            var list = codeLexem.Parse(CodeLanguage);
            foreach (CodeLexem current in list)
            {
                collection.Add(current.ToInline(CodeLanguage));
            }
        }

        public CodeLanguage CodeLanguage
        {
            get; set;
        }
    }
}