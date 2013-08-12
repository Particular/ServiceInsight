using System.Windows.Documents;
using Particular.ServiceInsight.Desktop.CodeParser;

namespace Particular.ServiceInsight.Desktop.MessageViewers.XmlViewer
{
    /// <summary>
    /// Interaction logic for XmlMessageView.xaml
    /// </summary>
    public partial class XmlMessageView : IXmlMessageView
    {
        public XmlMessageView()
        {
            InitializeComponent();
        }

        public virtual void Display(string message)
        {
            if (message == null)
                return;

            var presenter = new CodeBlockPresenter(CodeLanguage.Xml);
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
