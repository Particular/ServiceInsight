namespace ServiceInsight.MessageList
{
    using Caliburn.Micro;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Models;

    public class MessageSelectionContext : PropertyChangedBase
    {
        IEventAggregator eventAggregator;

        public MessageSelectionContext(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public StoredMessage SelectedMessage { get; set; }

        void OnSelectedMessageChanged()
        {
            eventAggregator.PublishOnUIThread(new SelectedMessageChanged());
        }
    }
}