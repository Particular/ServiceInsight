namespace Particular.ServiceInsight.Desktop.Framework.Commands
{
    using Particular.ServiceInsight.Desktop.Framework;
    using Particular.ServiceInsight.Desktop.Models;
    using Particular.ServiceInsight.Desktop.ServiceControl;

    class CopyMessageURICommand : BaseCommand
    {
        private readonly IClipboard clipboard;
        private readonly IServiceControl serviceControl;

        public CopyMessageURICommand(IClipboard clipboard, IServiceControl serviceControl)
        {
            this.clipboard = clipboard;
            this.serviceControl = serviceControl;
        }

        public override bool CanExecute(object parameter)
        {
            var message = parameter as StoredMessage;
            return message != null;
        }

        public override void Execute(object parameter)
        {
            var message = parameter as StoredMessage;
            if (message == null)
                return;

            clipboard.CopyTo(serviceControl.CreateServiceInsightUri(message).ToString());
        }
    }
}