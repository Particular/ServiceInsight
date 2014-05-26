namespace Particular.ServiceInsight.Desktop.CodeParser
{
    using System.Windows.Documents;
    using System.Windows.Media;

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

        public void FillInlines(string text, InlineCollection collection, Color? color = null)
        {
            text = text.Replace("\r", "");
            var codeLexem = new CodeLexem(text);
            var list = codeLexem.Parse(CodeLanguage);
            foreach (var current in list)
            {
                collection.Add(current.ToInline(CodeLanguage, color));
            }
        }

        public CodeLanguage CodeLanguage
        {
            get; set;
        }
    }
}