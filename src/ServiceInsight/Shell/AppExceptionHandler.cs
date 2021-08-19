namespace ServiceInsight.Shell
{
    using System;
    using System.Windows;
    using Caliburn.Micro;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.UI.ScreenManager;

    public class AppExceptionHandler
    {
        IServiceInsightWindowManager windowManager;
        IEventAggregator eventAggregator;
        ShellViewModel shell;

        public AppExceptionHandler(
            IServiceInsightWindowManager windowManager,
            IEventAggregator eventAggregator,
            ShellViewModel shell)
        {
            this.windowManager = windowManager;
            this.eventAggregator = eventAggregator;
            this.shell = shell;
        }

        public void Handle(Exception error, Action<Exception> baseAction)
        {
            var rootError = error.GetBaseException();

            StopAsyncProgress(rootError);

            if (IsSoftError(rootError))
            {
                ShowWarning(rootError);
            }
            else
            {
                baseAction(error);
            }
        }

        void StopAsyncProgress(Exception rootError)
        {
            if (shell.WorkInProgress)
            {
                eventAggregator.PublishOnUIThread(new AsyncOperationFailed(rootError.Message));
            }
        }

        bool IsSoftError(Exception rootError) => rootError is NotImplementedException;

        void ShowWarning(Exception error)
        {
            windowManager.ShowMessageBox(error.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}