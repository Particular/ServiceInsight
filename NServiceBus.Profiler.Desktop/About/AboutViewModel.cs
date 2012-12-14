using System.Reflection;
using Caliburn.PresentationFramework.Screens;
using ExceptionHandler;
using NServiceBus.Profiler.Common.ExtensionMethods;
using NServiceBus.Profiler.Core;

namespace NServiceBus.Profiler.Desktop.About
{
    public class AboutViewModel : Screen
    {
        private readonly INetworkOperations _networkOperations;
        private readonly Assembly _assembly;
        private readonly AssemblyCopyrightAttribute _copyright;
        private readonly AssemblyTitleAttribute _title;
        private readonly AssemblyInformationalVersionAttribute _version;
        private readonly SupportWebUrlAttribute _support;

        public AboutViewModel(INetworkOperations networkOperations)
        {
            _networkOperations = networkOperations;
            DisplayName = "About";
            _assembly = GetType().Assembly;
            _copyright = _assembly.GetAttribute<AssemblyCopyrightAttribute>();
            _title = _assembly.GetAttribute<AssemblyTitleAttribute>();
            _version = _assembly.GetAttribute<AssemblyInformationalVersionAttribute>();
            _support = _assembly.GetAttribute<SupportWebUrlAttribute>();
        }

        public string Copyright
        {
            get { return _copyright.Copyright; }
        }

        public string ApplicationName
        {
            get { return _title.Title; }
        }

        public string Version
        {
            get { return string.Format("Version {0}", _version.InformationalVersion); }
        }

        public string ProductUrl
        {
            get { return _support.WebUrl; }
        }
        
        public void NavigateToWebsite()
        {
            _networkOperations.Browse(ProductUrl);
        }
    }
}