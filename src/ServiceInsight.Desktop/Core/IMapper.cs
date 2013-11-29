using System.Messaging;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Core
{
    public interface IMapper
    {
        Queue MapQueue(MessageQueue source);
        MessageInfo MapInfo(Message source);
        MessageBody MapBody(Message source);
    }
}