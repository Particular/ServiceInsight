namespace ServiceInsight.Framework.Commands
{
    using Models;
    using ServiceInsight.MessageList;

    public class ChangeSelectedMessageCommand : BaseCommand
    {
        readonly MessageSelectionContext selection;

        public ChangeSelectedMessageCommand(MessageSelectionContext selectionContext)
        {
            selection = selectionContext;
        }

        public override bool CanExecute(object parameter)
        {
            return parameter is StoredMessage;
        }

        public override void Execute(object parameter)
        {
            selection.SelectedMessage = parameter as StoredMessage;
        }
    }
}