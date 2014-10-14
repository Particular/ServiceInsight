namespace Particular.ServiceInsight.Desktop.Common
{
    using Framework;
    using Models;

    class CopyConversationIDCommand : BaseCommand
    {
        private readonly IClipboard clipboard;

        public CopyConversationIDCommand(IClipboard clipboard)
        {
            this.clipboard = clipboard;
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