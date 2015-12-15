namespace ServiceInsight.Saga
{
    using System;
    using System.Windows.Input;
    using System.Windows.Media;
    using ICSharpCode.AvalonEdit.Folding;
    using ServiceInsight.MessageViewers.JsonViewer;

    public partial class SagaContentViewer
    {
        FoldingManager foldingManager;
        BraceFoldingStrategy foldingStrategy;

        public SagaContentViewer()
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

        SagaUpdatedValue Model
        {
            get { return (SagaUpdatedValue)DataContext; }
        }

        void OnCloseGlyphClicked(object sender, MouseButtonEventArgs e)
        {
            Model.MessageContentVisible = false;
        }
    }
}