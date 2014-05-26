namespace Particular.ServiceInsight.Desktop.Saga
{
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;
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

        public static readonly DependencyProperty MessageContentProperty = DependencyProperty.Register("MessageContent", typeof(SagaUpdatedValue), typeof(SagaContentViewer), new FrameworkPropertyMetadata(default(SagaUpdatedValue), OnMessageContentChanged));

        private static void OnMessageContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SagaContentViewer)d).OnMessageContentChanged(e.NewValue as SagaUpdatedValue);
        }

        public SagaUpdatedValue MessageContent
        {
            get { return (SagaUpdatedValue)GetValue(MessageContentProperty); }
            set { SetValue(MessageContentProperty, value); }
        }

        private void OnMessageContentChanged(SagaUpdatedValue model)
        {
            Clear();
            Display(model.EffectiveValue);
        }

        private void Display(string message)
        {
            if (message == null)
                return;

            var presenter = new CodeBlockPresenter(CodeLanguage.Plain);
            var paragraph = new Paragraph();

            presenter.FillInlines(message, paragraph.Inlines, Colors.White); //TODO: Do it as a dependency property?
            document.Document.Blocks.Add(paragraph);
        }

        private void Clear()
        {
            document.Document.Blocks.Clear();
        }
    }
}
