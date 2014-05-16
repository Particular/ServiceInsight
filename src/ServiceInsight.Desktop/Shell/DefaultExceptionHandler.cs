namespace Particular.ServiceInsight.Desktop.Shell
{
    using System;
    using System.Windows;
    using Caliburn.Micro;
    using Core.UI.ScreenManager;
    using Events;
    using ExceptionHandler;
    using ExceptionHandler.Wpf;

    public class DefaultExceptionHandler : WpfExceptionHandler
    {
        IWindowManagerEx windowManager;
        IEventAggregator eventAggregator;
        ShellViewModel shell;

        public DefaultExceptionHandler(
            IWindowManagerEx windowManager,
            IExceptionViewModel exceptionViewModel,
            IEventAggregator eventAggregator,
            ShellViewModel shell)
            : base(exceptionViewModel)
        {
            this.windowManager = windowManager;
            this.eventAggregator = eventAggregator;
            this.shell = shell;
        }

        public override void Handle(Exception error)
        {
            var rootError = error.GetBaseException();

            StopAsyncProgress(rootError);

            if (IsSoftError(rootError))
            {
                ShowWarning(rootError);
            }
            else
            {
                base.Handle(error);
            }
        }

        void StopAsyncProgress(Exception rootError)
        {
            if (shell.WorkInProgress)
            {
                eventAggregator.Publish(new AsyncOperationFailed(rootError.Message));
            }
        }

        bool IsSoftError(Exception rootError)
        {
            return rootError is NotImplementedException;
        }

        void ShowWarning(Exception error)
        {
            windowManager.ShowMessageBox(error.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}