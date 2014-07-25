namespace Particular.ServiceInsight.Desktop.Common
{
    using System;
    using System.Windows.Input;
    using Framework;
    using Models;

    class CopyConversationIDCommand : ICommand
    {
        private readonly IClipboard clipboard;

        public CopyConversationIDCommand(IClipboard clipboard)
        {
            this.clipboard = clipboard;
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

            clipboard.CopyTo(message.ConversationId);
        }
    }
}