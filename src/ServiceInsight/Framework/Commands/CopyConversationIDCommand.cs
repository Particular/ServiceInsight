namespace Particular.ServiceInsight.Desktop.Framework.Commands
{
    using Particular.ServiceInsight.Desktop.Framework;
    using Particular.ServiceInsight.Desktop.Models;

    class CopyConversationIDCommand : BaseCommand
    {
        private readonly IClipboard clipboard;

        public CopyConversationIDCommand(IClipboard clipboard)
        {
            this.clipboard = clipboard;
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

            clipboard.CopyTo(message.ConversationId);
        }
    }
}