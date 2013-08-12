using System;
using System.Windows;

namespace Particular.ServiceInsight.Desktop
{
    public interface IAppCommands
    {
        void ShutdownImmediately();
    }

    public class AppCommandsWrapper : IAppCommands
    {
        private readonly IAppCommands _current;

        public AppCommandsWrapper()
            : this((IAppCommands)Application.Current)
        {
        }

        public AppCommandsWrapper(IAppCommands app)
        {
            _current = app;
        }

        public void ShutdownImmediately()
        {
            if (_current != null)
            {
                _current.ShutdownImmediately();
            }
        }
    }

    public partial class App : IAppCommands
    {
        public App()
        {
            InitializeComponent();
        }

        public void ShutdownImmediately()
        {
            Environment.Exit(0);
        }
    }
}
