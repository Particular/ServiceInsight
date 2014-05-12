namespace Particular.ServiceInsight.Desktop.Shell
{
    using System;
    using System.Messaging;
    using System.Windows;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.ViewModels;
    using Core.UI.ScreenManager;
    using Events;
    using ExceptionHandler;
    using ExceptionHandler.Wpf;

    public class DefaultExceptionHandler : WpfExceptionHandler
    {
        private readonly IWindowManagerEx windowManager;
        private readonly IEventAggregator eventAggregator;
        private readonly IShellViewModel shell;

        public DefaultExceptionHandler(
            IWindowManagerEx windowManager, 
            IViewModelFactory screenFactory,
            IEventAggregator eventAggregator,
            IShellViewModel shell) 
            : base(screenFactory.Create<IExceptionViewModel>())
        {
            this.windowManager = windowManager;
            this.eventAggregator = eventAggregator;
            this.shell = shell;
        }

        public override void Handle(Exception error)
        {
            var rootError = error.GetBaseException();

            StopAsyncProgress(rootError);

            if (IsIgnoredError(rootError)) return;

            if(IsSoftError(rootError))
            {
                ShowWarning(rootError);
            }
            else
            {
                base.Handle(error);
            }
        }

        private void StopAsyncProgress(Exception rootError)
        {
            if (shell.WorkInProgress)
            {
                eventAggregator.Publish(new AsyncOperationFailed(rootError.Message));
            }
        }

        private bool IsIgnoredError(Exception rootError)
        {
            return rootError is MessageQueueException;
        }

        private bool IsSoftError(Exception rootError)
        {
            return rootError is NotImplementedException;
        }

        private void ShowWarning(Exception error)
        {
            windowManager.ShowMessageBox(error.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}