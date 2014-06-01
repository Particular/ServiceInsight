namespace Particular.ServiceInsight.Desktop.Saga
{
    using System.Windows.Documents;
    using System.Windows.Input;
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
            Loaded += (s, e) => OnViewLoaded();
        }

        SagaUpdatedValue Model
        {
            get { return DataContext as SagaUpdatedValue; }
        }

        void OnViewLoaded()
        {
            Clear();
            Display(Model.EffectiveValue);
        }

        void Display(string message)
        {
            if (message == null)
                return;

            var presenter = new CodeBlockPresenter(CodeLanguage.Plain);
            var paragraph = new Paragraph();

            presenter.FillInlines(message, paragraph.Inlines, Colors.White); //TODO: Do it as a dependency property?
            document.Document.Blocks.Add(paragraph);
        }

        void Clear()
        {
            document.Document.Blocks.Clear();
        }

        void OnCloseGlyphClicked(object sender, MouseButtonEventArgs e)
        {
            Model.MessageContentVisible = false;
        }
    }
}
