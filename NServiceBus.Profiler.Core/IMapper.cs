using System.Messaging;
using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Core
{
    public interface IMapper
    {
        Queue MapQueue(MessageQueue source);
        MessageInfo MapInfo(Message source);
        MessageBody MapBody(Message source);
    }
}