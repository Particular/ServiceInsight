namespace Particular.ServiceInsight.Desktop.Saga
{
    using System.Windows;
    using System.Windows.Media;
    using ICSharpCode.AvalonEdit.Folding;
    using Particular.ServiceInsight.Desktop.MessageViewers.JsonViewer;

    public partial class SagaContentViewer
    {
        FoldingManager foldingManager;

        public SagaContentViewer()
        {
            InitializeComponent();

            SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);
            foldingManager = FoldingManager.Install(document.TextArea);
            document.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();

            Loaded += OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var syntaxHighlighting = ((ContentViewer) DataContext).SyntaxHighlighting;

            if (syntaxHighlighting == "XML")
            {
                var foldingStrategy = new XmlFoldingStrategy();
                foldingStrategy.UpdateFoldings(foldingManager, document.Document);
            }
            else
            {
                var foldingStrategy = new BraceFoldingStrategy();
                foldingStrategy.UpdateFoldings(foldingManager, document.Document);
            }
        }

        void OnCloseGlyphClicked(object sender, RoutedEventArgs routedEventArgs)
        {
            ((ContentViewer)DataContext).Visible = false;
        }
    }
}