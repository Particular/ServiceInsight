using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Caliburn.PresentationFramework.ApplicationModel;
using NServiceBus.Profiler.Common.ExtensionMethods;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Core.MessageDecoders;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Properties;

namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    public class ErrorHeaderViewModel : HeaderInfoViewModelBase, IErrorHeaderDisplay
    {
        public ErrorHeaderViewModel(
            IEventAggregator eventAggregator, 
            IContentDecoder<IList<HeaderInfo>> decoder, 
            IQueueManagerAsync queueManager) 
            : base(eventAggregator, decoder, queueManager)
        {
            DisplayName = "Errors";
        }

        public virtual bool CanReturnToSource()
        {
            return SelectedQueue != null &&
                   !SelectedQueue.IsRemoteQueue() &&
                   SelectedMessage != null &&
                   Items.Count > 0 &&
                   Items.Any(header => header.Key.Equals(HeaderInfo.FailedQueueKey, StringComparison.InvariantCultureIgnoreCase));
        }

        public virtual void ReturnToSource()
        {
            if (!CanReturnToSource()) return;

            var destinationQueueName = Items.First(header => header.Key == HeaderInfo.FailedQueueKey);
            var destinationAddress = Address.Parse(destinationQueueName.Value);
            var queues = QueueManager.GetQueues();
            var destinationQueue = queues.FirstOrDefault(q => q.Address == destinationAddress);

            if (destinationQueue != null)
            {
                QueueManager.MoveMessage(SelectedQueue, destinationQueue, SelectedMessage.Id);
                EventAggregator.Publish(new MessageRemovedFromQueue { Message = SelectedMessage });
            }
        }

        public override ImageSource GroupImage
        {
            get { return Resources.HeaderError.ToBitmapImage(); }
        }

        protected override bool IsMatchingHeader(HeaderInfo header)
        {
            return header.Key.StartsWith("NServiceBus.ExceptionInfo", StringComparison.OrdinalIgnoreCase) ||
                   header.Key.EndsWith("FailedQ", StringComparison.OrdinalIgnoreCase)                     ||
                   header.Key.EndsWith("TimeOfFailure", StringComparison.OrdinalIgnoreCase);
        }
    }
}