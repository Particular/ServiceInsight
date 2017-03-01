namespace ServiceInsight.MessagePayloadViewer
{
    using System;
    using System.Windows.Media;
    using ICSharpCode.AvalonEdit.Folding;
    using ServiceInsight.MessageViewers.JsonViewer;

    /// <summary>
    /// Interaction logic for MessagePayloadView.xaml
    /// </summary>
    public partial class MessagePayloadView
    {
        FoldingManager foldingManager;
        BraceFoldingStrategy foldingStrategy;

        public MessagePayloadView()
        {
            InitializeComponent();

            foldingManager = FoldingManager.Install(document.TextArea);
            foldingStrategy = new BraceFoldingStrategy();
            SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);
            document.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
            document.TextChanged += DocumentOnTextChanged;
        }

        void DocumentOnTextChanged(object sender, EventArgs eventArgs)
        {
            foldingStrategy.UpdateFoldings(foldingManager, document.Document);
        }
    }
}
