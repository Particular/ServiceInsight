namespace ServiceInsight.MessageList
{
    using System;
    using Framework;
    using Pirac;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Models;

    public class MessageSelectionContext : BindableObject
    {
        public MessageSelectionContext(IRxEventAggregator eventAggregator)
        {
            this.WhenPropertyChanged(nameof(SelectedMessage))
                .Subscribe(_ => eventAggregator.Publish(new SelectedMessageChanged()));
        }

        public StoredMessage SelectedMessage { get; set; }
    }
}