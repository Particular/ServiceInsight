namespace ServiceInsight.Shell
{
    using System;
    using System.Windows;
    using Framework;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.UI.ScreenManager;

    public class AppExceptionHandler
    {
        IWindowManagerEx windowManager;
        IRxEventAggregator eventAggregator;
        ShellViewModel shell;

        public AppExceptionHandler(
            IWindowManagerEx windowManager,
            IRxEventAggregator eventAggregator,
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
                eventAggregator.Publish(new AsyncOperationFailed(rootError.Message));
            }
        }

        bool IsSoftError(Exception rootError) => rootError is NotImplementedException;

        void ShowWarning(Exception error)
        {
            windowManager.ShowMessageBox(error.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}