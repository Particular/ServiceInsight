using System.Windows.Media;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Indentation;
using ServiceInsight.MessageViewers;
using ServiceInsight.Models;

namespace ServiceInsight.CustomViewer.Plugin
{
    public partial class MyCustomDecryptionView : IDisplayMessageBody
    {
        readonly FoldingManager foldingManager;
        readonly XmlFoldingStrategy foldingStrategy;

        public MyCustomDecryptionView()
        {
            InitializeComponent();
            foldingManager = FoldingManager.Install(document.TextArea);
            foldingStrategy = new XmlFoldingStrategy();
            SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);
            document.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
        }
        
        public virtual void Display(StoredMessage message)
        {
            document.Document.Text = message?.Body?.Text;
            foldingStrategy.UpdateFoldings(foldingManager, document.Document);
        }

        public virtual void Clear()
        {
            document.Document.Text = string.Empty;
            foldingStrategy.UpdateFoldings(foldingManager, document.Document);
        }
    }
}
