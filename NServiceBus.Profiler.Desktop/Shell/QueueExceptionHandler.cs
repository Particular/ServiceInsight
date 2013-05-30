using System;
using System.Messaging;
using System.Windows;
using Caliburn.PresentationFramework.ViewModels;
using ExceptionHandler.Wpf;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Core.Licensing;
using NServiceBus.Profiler.Desktop.ScreenManager;
using Rhino.Licensing;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public class DefaultExceptionHandler : WpfExceptionHandler
    {
        private readonly IWindowManagerEx _windowManager;
        private readonly IAppCommands _appCommands;

        public DefaultExceptionHandler(
            IWindowManagerEx windowManager, 
            IViewModelFactory screenFactory,
            IAppCommands appCommands) 
            : base(windowManager, screenFactory)
        {
            _windowManager = windowManager;
            _appCommands = appCommands;
        }

        public override void Handle(Exception error)
        {
            var rootError = error.GetBaseException();
            if(IsSoftError(rootError))
            {
                ShowWarning(rootError);
            }
            else if (IsHardError(rootError))
            {
                ShowError(rootError);
                _appCommands.ShutdownImmediately();
            }
            else
            {
                base.Handle(error);
            }
        }

        private bool IsHardError(Exception rootError)
        {
            return rootError is LicenseExpiredException  ||
                   rootError is LicenseNotFoundException ||
                   rootError is InvalidLicenseException  ||
                   rootError is LicenseFileNotFoundException;
        }

        private bool IsSoftError(Exception rootError)
        {
            return rootError is MessageQueueException ||
                   rootError is QueueManagerException ||
                   rootError is NotImplementedException;
        }

        private void ShowWarning(Exception error)
        {
            _windowManager.ShowMessageBox(error.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ShowError(Exception error)
        {
            _windowManager.ShowMessageBox(error.Message, "Fatal", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}