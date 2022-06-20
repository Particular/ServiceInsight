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

        public bool SearchInProgress { get; set; }

#pragma warning disable IDE0051 // Remove unused private members
        void OnSelectedMessageChanged()
#pragma warning restore IDE0051 // Remove unused private members
        {
            eventAggregator.PublishOnUIThread(new SelectedMessageChanged());
        }
    }
}