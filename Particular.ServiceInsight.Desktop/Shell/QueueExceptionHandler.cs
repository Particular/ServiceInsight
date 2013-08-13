﻿using System;
using System.Messaging;
using System.Windows;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.ViewModels;
using ExceptionHandler.Wpf;
using Particular.ServiceInsight.Desktop.Core;
using Particular.ServiceInsight.Desktop.Core.Licensing;
using Particular.ServiceInsight.Desktop.Events;
using Particular.ServiceInsight.Desktop.ScreenManager;
using Particular.ServiceInsight.Desktop.Shell;
using Rhino.Licensing;

namespace Particular.ServiceInsight.Desktop.Shell
{
    public class DefaultExceptionHandler : WpfExceptionHandler
    {
        private readonly IWindowManagerEx _windowManager;
        private readonly IAppCommands _appCommands;
        private readonly IEventAggregator _eventAggregator;
        private readonly IShellViewModel _shell;

        public DefaultExceptionHandler(
            IWindowManagerEx windowManager, 
            IViewModelFactory screenFactory,
            IAppCommands appCommands, 
            IEventAggregator eventAggregator,
            IShellViewModel shell) 
            : base(windowManager, screenFactory)
        {
            _windowManager = windowManager;
            _appCommands = appCommands;
            _eventAggregator = eventAggregator;
            _shell = shell;
        }

        public override void Handle(Exception error)
        {
            var rootError = error.GetBaseException();

            StopAsyncProgress(rootError);

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

        private void StopAsyncProgress(Exception rootError)
        {
            if (_shell.WorkInProgress)
            {
                _eventAggregator.Publish(new AsyncOperationFailedEvent {Message = rootError.Message});
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