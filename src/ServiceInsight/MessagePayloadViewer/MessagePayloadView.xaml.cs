namespace ServiceInsight.MessagePayloadViewer
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using ICSharpCode.AvalonEdit.Folding;
    using ServiceInsight.Framework.Settings;
    using ServiceInsight.MessageViewers.JsonViewer;
    using ServiceInsight.Shell;

    /// <summary>
    /// Interaction logic for MessagePayloadView.xaml
    /// </summary>
    public partial class MessagePayloadView : IPersistableLayout
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

        public void OnSaveLayout(ISettingsProvider settingsProvider)
        {
            var settings = new MessagePayloadViewSetting
            {
                Left = Left,
                Top = Top,
                Width = ActualWidth,
                Height = ActualHeight,
                WindowState = WindowState
            };

            settingsProvider.SaveSettings(settings);
        }

        public void OnRestoreLayout(ISettingsProvider settingsProvider)
        {
            var settings = settingsProvider.GetSettings<MessagePayloadViewSetting>();

            Height = settings.Height;
            Width = settings.Width;

            if (settings.WindowState == WindowState.Normal)
            {
                Left = settings.Left;
                Top = settings.Top;
            }

            WindowState = settings.WindowState;
        }

        public void OnResetLayout(ISettingsProvider settingsProvider)
        {
        }
    }

    public class MessagePayloadViewSetting : WindowStateSetting
    {
    }
}
