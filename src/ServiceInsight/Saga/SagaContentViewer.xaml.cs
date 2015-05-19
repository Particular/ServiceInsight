namespace Particular.ServiceInsight.Desktop.Saga
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;
    using ICSharpCode.AvalonEdit.Folding;
    using ICSharpCode.AvalonEdit.Highlighting;
    using Particular.ServiceInsight.Desktop.MessageViewers.JsonViewer;

    public partial class SagaContentViewer
    {
        FoldingManager foldingManager;

        public SagaContentViewer()
        {
            RegisterDarkThemeHighlighters();

            InitializeComponent();

            SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);
            foldingManager = FoldingManager.Install(document.TextArea);
            document.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
            document.Options.EnableHyperlinks = false;
            document.Options.EnableEmailHyperlinks = false;

            Loaded += OnLoaded;
        }

        static void RegisterDarkThemeHighlighters()
        {
            var highlighters = new List<string[]> { new[] { "JavaScript-Mode.xshd", "JavaScript", ".js" }, new[] { "XML-Mode.xshd", "XML", "xml" } };

            foreach (var highlighter in highlighters)
            {
                using (var stream = typeof(SagaContentViewer).Assembly.GetManifestResourceStream("ServiceInsight.Saga.Resources." + highlighter[0]))
                {
                    using (XmlReader reader = new XmlTextReader(stream))
                    {
                        var customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);

                        HighlightingManager.Instance.RegisterHighlighting(highlighter[1], new[]
                        {
                            highlighter[2]
                        }, customHighlighting);
                    }
                }
            }
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