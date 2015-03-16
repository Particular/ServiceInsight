namespace Particular.ServiceInsight.Desktop.CodeParser
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    public class CodeTextBox : RichTextBox
    {
        public static readonly DependencyProperty CodeProperty = DependencyProperty.Register(
            "Code", typeof(string), typeof(CodeTextBox), new FrameworkPropertyMetadata("", UpdateCode));

        public string Code
        {
            get { return (string)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        public static readonly DependencyProperty CodeLanguageProperty = DependencyProperty.Register(
            "CodeLanguage", typeof(CodeLanguage), typeof(CodeTextBox), new FrameworkPropertyMetadata(CodeLanguage.Plain, UpdateCode));

        public CodeLanguage CodeLanguage
        {
            get { return (CodeLanguage)GetValue(CodeLanguageProperty); }
            set { SetValue(CodeLanguageProperty, value); }
        }

        static void UpdateCode(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeTextBox)d).UpdateCode();
        }

        private void UpdateCode()
        {
            Document.Blocks.Clear();

            var presenter = new CodeBlockPresenter(CodeLanguage);
            var paragraph = new Paragraph();

            presenter.FillInlines(Code, paragraph.Inlines, Foreground);
            Document.Blocks.Add(paragraph);
        }
    }
}