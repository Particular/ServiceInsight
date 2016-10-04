namespace ServiceInsight.MessageViewers.XmlViewer
{
    using System.Windows.Media;
    using System.Xml;
    using ICSharpCode.AvalonEdit.Folding;
    using ServiceInsight.ExtensionMethods;

    public partial class XmlMessageView : IMessageView
    {
        FoldingManager foldingManager;
        XmlFoldingStrategy foldingStrategy;

        public XmlMessageView()
        {
            InitializeComponent();
            foldingManager = FoldingManager.Install(document.TextArea);
            foldingStrategy = new XmlFoldingStrategy();
            SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);
            document.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
        }

        public virtual void Display(string message)
        {
            if (message == null)
            {
                return;
            }

            var doc = new XmlDocument();
            var text = message;
            try
            {
                doc.LoadXml(message);
                text = doc.GetFormatted();
            }
            catch (XmlException)
            {
                // It looks like we having issues parsing the xml
                // Best to do in this circunstances is to still display the text
            }

            document.Document.Text = text;
            foldingStrategy.UpdateFoldings(foldingManager, document.Document);
        }

        public virtual void Clear()
        {
            document.Document.Text = string.Empty;
            foldingStrategy.UpdateFoldings(foldingManager, document.Document);
        }
    }
}