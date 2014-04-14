using NServiceBus.Profiler.Desktop.Core.Settings;
using NServiceBus.Profiler.Desktop.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
    /// <summary>
    /// Interaction logic for ExceptionDetailView.xaml
    /// </summary>
    public partial class ExceptionDetailView : Window, IPersistableLayout
    {
        public ExceptionDetailView()
        {
            InitializeComponent();
        }

        public void OnSaveLayout(ISettingsProvider settingsProvider)
        {
            var settings = new ExceptionViewSettings
                                {
                                    Left = this.Left, Top = this.Top,
                                    Width = this.ActualWidth, Height = this.ActualHeight,
                                    WindowState = this.WindowState
                                };
            settingsProvider.SaveSettings(settings);
        }

        public void OnRestoreLayout(ISettingsProvider settingsProvider)
        {
            var settings = settingsProvider.GetSettings<ExceptionViewSettings>();

            this.Height = settings.Height;
            this.Width = settings.Width;

            if (settings.WindowState == System.Windows.WindowState.Normal)
            {
                this.Left = settings.Left;
                this.Top = settings.Top;
            }

            this.WindowState = settings.WindowState;
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
