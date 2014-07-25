namespace Particular.ServiceInsight.Desktop.Common
{
    using System;
    using System.Windows.Input;
    using Framework;
    using Models;
    using ServiceControl;

    class CopyMessageURICommand : ICommand
    {
        private readonly IClipboard clipboard;
        private readonly IServiceControl serviceControl;

        public CopyMessageURICommand(IClipboard clipboard, IServiceControl serviceControl)
        {
            this.clipboard = clipboard;
            this.serviceControl = serviceControl;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var message = parameter as StoredMessage;
            if (message == null)
                return;

            clipboard.CopyTo(serviceControl.CreateServiceInsightUri(message).ToString());
        }
    }
}