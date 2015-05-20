namespace Particular.ServiceInsight.Desktop.MessageViewers.JsonViewer
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows.Media;
    using ICSharpCode.AvalonEdit.Folding;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

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
            document.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
        }

        public virtual void Display(string message)
        {
             if (message == null)
                return;
            
            var sb = new StringBuilder();
            using (var writer = new JsonTextWriter(new StringWriter(sb)))
            {
                writer.Formatting = Formatting.Indented;
                try
                {
                    var obj = JObject.Parse(message);
                    obj.WriteTo(writer);
                }
                catch (JsonReaderException)
                {
                    // It looks like we cannot display json, moving on
                    return;
                }
            }
            
            document.Document.Text = sb.ToString();
            foldingStrategy.UpdateFoldings(foldingManager, document.Document);
        }

        public virtual void Clear()
        {
            document.Document.Text = String.Empty;
            foldingStrategy.UpdateFoldings(foldingManager, document.Document);
        }
    }
}
