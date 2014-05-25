namespace Particular.ServiceInsight.Desktop.Saga
{
    using System.Windows;
    using System.Windows.Documents;
    using CodeParser;

    /// <summary>
    /// Interaction logic for SagaContentViewer.xaml
    /// </summary>
    public partial class SagaContentViewer
    {
        public SagaContentViewer()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MessageContentProperty = DependencyProperty.Register("MessageContent", typeof(string), typeof(SagaContentViewer), new FrameworkPropertyMetadata(default(string), OnMessageContentChanged));

        private static void OnMessageContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SagaContentViewer)d).OnMessageContentChanged(e.NewValue as string);
        }

        public string MessageContent
        {
            get { return (string)GetValue(MessageContentProperty); }
            set { SetValue(MessageContentProperty, value); }
        }

        private void OnMessageContentChanged(string newContent)
        {
            Clear();
            Display(newContent);
        }

        private void Display(string message)
        {
            if (message == null)
                return;

            var presenter = new CodeBlockPresenter(CodeLanguage.Plain);
            var paragraph = new Paragraph();

            presenter.FillInlines(message, paragraph.Inlines);
            document.Document.Blocks.Add(paragraph);
        }

        private void Clear()
        {
            document.Document.Blocks.Clear();
        }
    }
}
