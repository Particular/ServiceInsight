using System.Windows.Input;
using Caliburn.PresentationFramework.Screens;
using ExceptionHandler;
using NServiceBus.Profiler.Common.ExtensionMethods;
using NServiceBus.Profiler.Core;

namespace NServiceBus.Profiler.Desktop.Shell
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class AboutView
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private AboutViewModel Model 
        {
            get { return (AboutViewModel) DataContext; }
        }

        private void OnImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            Model.NavigateToSite();
        }
    }

    public class AboutViewModel : Screen
    {
        private readonly INetworkOperations _networkOperations;

        public AboutViewModel(INetworkOperations networkOperations)
        {
            _networkOperations = networkOperations;
            DisplayName = "About";
        }

        public void NavigateToSite()
        {
            var supportUrl = GetType().Assembly.GetAttribute<SupportWebUrlAttribute>();
            _networkOperations.Browse(supportUrl.WebUrl);
        }
    }
}
