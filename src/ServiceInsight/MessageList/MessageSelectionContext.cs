namespace Particular.ServiceInsight.Desktop.MessageList
{
    using Caliburn.Micro;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using Particular.ServiceInsight.Desktop.Models;

    public class MessageSelectionContext : PropertyChangedBase
    {
        private IEventAggregator eventAggregator;
        private StoredMessage selectedMessage;

        public MessageSelectionContext(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public StoredMessage SelectedMessage
        {
            get { return selectedMessage; }
            set
            {
                if (selectedMessage != value)
                {
                    selectedMessage = value;
                    NotifyOfPropertyChange(nameof(SelectedMessage));
                    OnSelectedMessageChanged();
                }
            }
        }

        private void OnSelectedMessageChanged()
        {
            eventAggregator.Publish(new SelectedMessageChanged());
        }
    }
}