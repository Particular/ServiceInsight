using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Caliburn.PresentationFramework.ApplicationModel;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Core.MessageDecoders;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.ExtensionMethods;
using NServiceBus.Profiler.Desktop.MessageHeaders.Editors;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    [TypeConverter(typeof(HeaderInfoTypeConverter))]
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
                EventAggregator.Publish(new MessageRemovedFromQueue { Message = SelectedMessage });
            }
        }

        [Editor(typeof(ResizableDropDownEditor), typeof(ResizableDropDownEditor))]
        [Description("Stack trace for the error")]
        public string ExceptionInfo { get; set; }

        [Description("Failed Queue")]
        public string FailedQueue { get; set; }

        [Description("The first time the message has failed")]
        public string TimeOfFailure { get; set; }

        protected override void MapHeaderKeys()
        {
            ConditionsMap.Add(h => h.Key.StartsWith("NServiceBus.ExceptionInfo", StringComparison.OrdinalIgnoreCase), h => ExceptionInfo = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith("FailedQ", StringComparison.OrdinalIgnoreCase), h => FailedQueue = h.Value);
            ConditionsMap.Add(h => h.Key.EndsWith("TimeOfFailure", StringComparison.OrdinalIgnoreCase), h => TimeOfFailure = h.Value.ParseHeaderDate().ToString());
        }

        protected override void ClearHeaderValues()
        {
            FailedQueue = null;
            ExceptionInfo = null;
            TimeOfFailure = null;
        }
    }
}