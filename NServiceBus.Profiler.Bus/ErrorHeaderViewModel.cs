using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Views;
using ExceptionHandler;
using NServiceBus.Profiler.Bus.Properties;
using NServiceBus.Profiler.Common;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Common.Plugins;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Core.MessageDecoders;
using NServiceBus.Profiler.Common.ExtensionMethods;

namespace NServiceBus.Profiler.Bus
{
    [View(typeof(HeaderInfoView))]
    public class ErrorHeaderViewModel : HeaderInfoViewModelBase
    {
        public ErrorHeaderViewModel(
            IEventAggregator eventAggregator, 
            IContentDecoder<IList<HeaderInfo>> decoder, 
            IQueueManagerAsync queueManager, 
            IClipboard clipboard) 
            : base(eventAggregator, decoder, queueManager, clipboard)
        {
            DisplayName = "Errors";
            ContextMenuItems.AddRange(new[]
            {
                new PluginContextMenu("ReturnToSourceQueue", new RelayCommand(ReturnToSource, CanReturnToSource), 90)
                {
                    DisplayName = "Return To Source Queue",
                    Image = Resources.MessageReturn
                },
                new PluginContextMenu("CopyHeaderInfo", new RelayCommand(CopyHeaderInfo, CanCopyHeaderInfo))
                {
                    DisplayName = "Copy Header Info",
                }
            });
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
                EventAggregator.Publish(new MessageRemovedFromQueueEvent { Message = SelectedMessage });
            }
        }

        protected override bool IsMatchingHeader(HeaderInfo header)
        {
            return header.Key.StartsWith("NServiceBus.ExceptionInfo", StringComparison.OrdinalIgnoreCase) ||
                   header.Key.EndsWith("FailedQ", StringComparison.OrdinalIgnoreCase)                     ||
                   header.Key.EndsWith("TimeOfFailure", StringComparison.OrdinalIgnoreCase);
        }
    }
}