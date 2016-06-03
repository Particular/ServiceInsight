namespace ServiceInsight.MessageList
{
    using Caliburn.Micro;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Models;

    public class MessageSelectionContext : PropertyChangedBase
    {
        IEventAggregator eventAggregator;
        StoredMessage selectedMessage;

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

        void OnSelectedMessageChanged()
        {
            eventAggregator.PublishOnUIThread(new SelectedMessageChanged());
        }
    }
}