namespace Particular.ServiceInsight.Desktop.MessageViewers.XmlViewer
{
    using System;
    using System.Windows.Media;
    using ICSharpCode.AvalonEdit.Folding;

    public partial class XmlMessageView : IXmlMessageView
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
                return;

            document.Document.Text = message;
            foldingStrategy.UpdateFoldings(foldingManager, document.Document);
        }

        public virtual void Clear()
        {
            document.Document.Text = String.Empty;
            foldingStrategy.UpdateFoldings(foldingManager, document.Document);
        }
    }
}
