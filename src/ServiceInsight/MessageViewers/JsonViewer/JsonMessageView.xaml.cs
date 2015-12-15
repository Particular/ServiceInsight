namespace ServiceInsight.MessageViewers.JsonViewer
{
    using System;
    using System.Windows.Media;
    using ICSharpCode.AvalonEdit.Folding;
    using ICSharpCode.AvalonEdit.Indentation;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using ServiceInsight.ExtensionMethods;

    public partial class JsonMessageView : IJsonMessageView
    {
        FoldingManager foldingManager;
        BraceFoldingStrategy foldingStrategy;

        public JsonMessageView()
        {
            InitializeComponent();
            foldingManager = FoldingManager.Install(document.TextArea);
            foldingStrategy = new BraceFoldingStrategy();
            SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);
            document.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
        }

        public virtual void Display(string message)
        {
            if (message == null)
            {
                return;
            }

            var text = message;
            try
            {
                var jObject = JObject.Parse(message);
                text = jObject.GetFormatted();
            }
            catch (JsonReaderException)
            {
                // It looks like we having issues parsing the json
                // Best to do in this circunstances is to still display the text
            }

            document.Document.Text = text;
            foldingStrategy.UpdateFoldings(foldingManager, document.Document);
        }

        public virtual void Clear()
        {
            document.Document.Text = String.Empty;
            foldingStrategy.UpdateFoldings(foldingManager, document.Document);
        }
    }
}