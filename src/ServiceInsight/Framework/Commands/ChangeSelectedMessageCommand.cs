namespace Particular.ServiceInsight.Desktop.Framework.Commands
{
    using Models;
    using Particular.ServiceInsight.Desktop.MessageList;

    public class ChangeSelectedMessageCommand : BaseCommand
    {
        readonly MessageSelectionContext selection;

        public ChangeSelectedMessageCommand(MessageSelectionContext selectionContext)
        {
            this.selection = selectionContext;
        }

        public override bool CanExecute(object parameter)
        {
            var msg = parameter as StoredMessage;
            return msg != null;
        }

        public override void Execute(object parameter)
        {
            selection.SelectedMessage = parameter as StoredMessage;
        }
    }
}