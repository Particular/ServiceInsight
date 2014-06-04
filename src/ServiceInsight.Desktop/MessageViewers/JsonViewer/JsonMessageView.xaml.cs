namespace Particular.ServiceInsight.Desktop.MessageViewers.JsonViewer
{
    using System.Windows.Documents;
    using CodeParser;

    public partial class JsonMessageView : IJsonMessageView
    {
        public JsonMessageView()
        {
            InitializeComponent();
        }

        public virtual void Display(string message)
        {
             if (message == null)
                return;

            var presenter = new CodeBlockPresenter(CodeLanguage.Json);
            var paragraph = new Paragraph();

            presenter.FillInlines(message, paragraph.Inlines);
            document.Document.Blocks.Add(paragraph);
        }

        public virtual void Clear()
        {
            document.Document.Blocks.Clear();
        }

    }
}
