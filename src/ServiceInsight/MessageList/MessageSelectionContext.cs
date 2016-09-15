namespace ServiceInsight.MessageList
{
    using Caliburn.Micro;
    using Framework;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Models;

    public class MessageSelectionContext : PropertyChangedBase
    {
        IRxEventAggregator eventAggregator;
        StoredMessage selectedMessage;

        public MessageSelectionContext(IRxEventAggregator eventAggregator)
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
            eventAggregator.Publish(new SelectedMessageChanged());
        }
    }
}