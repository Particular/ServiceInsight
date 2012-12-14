using System;
using System.Messaging;
using System.Windows;
using Caliburn.PresentationFramework.ViewModels;
using ExceptionHandler.Wpf;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Desktop.ScreenManager;

namespace NServiceBus.Profiler.Desktop.Shell
{
    public class DefaultExceptionHandler : WpfExceptionHandler
    {
        private readonly IWindowManagerEx _windowManager;

        public DefaultExceptionHandler(IWindowManagerEx windowManager, IViewModelFactory screenFactory) 
            : base(windowManager, screenFactory)
        {
            _windowManager = windowManager;
        }

        public override void Handle(Exception error)
        {
            var rootError = error.GetBaseException();
            if(IsSoftError(rootError))
            {
                ShowWarning(rootError);
            }
            else
            {
                base.Handle(error);
            }
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
    }
}