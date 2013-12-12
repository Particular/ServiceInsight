using System;
using System.Messaging;
using NServiceBus.Profiler.Desktop.ExtensionMethods;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Core
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
                BodyRaw = source.BodyStream.GetAsBytes(),
                HeaderRaw = source.Extension,
            };

            return m;
        }
    }
}