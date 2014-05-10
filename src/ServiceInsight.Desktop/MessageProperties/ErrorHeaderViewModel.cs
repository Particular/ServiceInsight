namespace Particular.ServiceInsight.Desktop.MessageProperties
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Core;
    using Core.MessageDecoders;
    using ExtensionMethods;
    using Models;

    public class ErrorHeaderViewModel : HeaderInfoViewModelBase, IErrorHeaderViewModel
    {
        public ErrorHeaderViewModel(
            IEventAggregator eventAggregator, 
            IContentDecoder<IList<HeaderInfo>> decoder, 
            IQueueManager queueManager) 
            : base(eventAggregator, decoder, queueManager)
        {
            DisplayName = "Errors";
        }

        public virtual bool CanReturnToSource()
        {
            return SelectedQueue != null &&
                   !SelectedQueue.IsRemoteQueue() &&
                   SelectedMessage != null &&
                   FailedQueue != null;
        }

        public virtual void ReturnToSource()
        {
            if (!CanReturnToSource()) return;

            var destinationAddress = Address.Parse(FailedQueue);
            var queues = QueueManager.GetQueues();
            var destinationQueue = queues.FirstOrDefault(q => q.Address == destinationAddress);

            if (destinationQueue != null)
            {
                QueueManager.MoveMessage(SelectedQueue, destinationQueue, SelectedMessage.Id);
            }
        }

        [Description("Stack trace for the error")]
        public string ExceptionInfo { get; private set; }

        [Description("Failed Queue")]
        public string FailedQueue { get; private set; }

        [Description("The first time the message has failed")]
        public string TimeOfFailure { get; private set; }

        protected override void MapHeaderKeys()
        {
            ConditionsMap.Add(h => h.Key.EndsWith(MessageHeaderKeys.ExceptionStackTrace, StringComparison.OrdinalIgnoreCase), h => ExceptionInfo = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith(MessageHeaderKeys.FailedQueue, StringComparison.OrdinalIgnoreCase), h => FailedQueue = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith(MessageHeaderKeys.TimeOfFailure, StringComparison.OrdinalIgnoreCase), h => TimeOfFailure = h.Value.ParseHeaderDate().ToString());
        }

        protected override void ClearHeaderValues()
        {
            FailedQueue = null;
            ExceptionInfo = null;
            TimeOfFailure = null;
        }
    }
}