using System;
using System.Messaging;
using NServiceBus.Profiler.Common.ExtensionMethods;
using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Core
{
    public class DefaultMapper : IMapper
    {
        public Queue MapQueue(MessageQueue queue)
        {
            if (queue == null) return null;

            var address = Address.ParseFormatName(queue.Path);
            Func<bool> isTransactional = () => queue.Transactional;
            Func<bool> canRead = () => queue.CanRead;

            var q = new Queue(address)
            {
                IsTransactional = isTransactional.TryGetValue(),
                CanRead = canRead.TryGetValue(),
            };

            return q;
        }

        public MessageInfo MapInfo(Message source)
        {
            return new MessageInfo(source.Id, source.Label, source.SentTime);
        }

        public MessageBody MapBody(Message source)
        {
            var m = new MessageBody(source.Id, source.Label, source.SentTime)
            {
                Content = source.BodyStream.GetAsBytes(),
                Headers = source.Extension,
                CorrelationId = source.CorrelationId,
                TransactionId = source.TransactionId,
                Destination = source.DestinationQueue == null ? null : MapQueue(source.DestinationQueue),
                Response = source.ResponseQueue == null ? null : MapQueue(source.ResponseQueue),
            };

            return m;
        }
    }
}