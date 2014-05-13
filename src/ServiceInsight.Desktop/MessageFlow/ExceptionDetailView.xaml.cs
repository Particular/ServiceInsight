namespace Particular.ServiceInsight.Desktop.MessageFlow
{
    using System.ComponentModel;
    using System.Windows;
    using Core.Settings;
    using Shell;

    public partial class ExceptionDetailView : IPersistableLayout
    {
        public ExceptionDetailView()
        {
            InitializeComponent();
        }

        public void OnSaveLayout(ISettingsProvider settingsProvider)
        {
            var settings = new ExceptionViewSettings
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
            var settings = settingsProvider.GetSettings<ExceptionViewSettings>();

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

    public class ExceptionViewSettings
    {
        [DefaultValue(double.NaN)]
        public double Left { get; set; }

        [DefaultValue(double.NaN)]
        public double Top { get; set; }

        [DefaultValue(450)]
        public double Width { get; set; }

        [DefaultValue(400)]
        public double Height { get; set; }

        [DefaultValue(typeof(WindowState), "Normal")]
        public WindowState WindowState { get; set; }
    }
}
