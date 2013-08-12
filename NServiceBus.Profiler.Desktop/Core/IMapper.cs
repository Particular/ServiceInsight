using System.Messaging;
using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.Core
{
    public interface IMapper
    {
        Queue MapQueue(MessageQueue source);
        MessageInfo MapInfo(Message source);
        MessageBody MapBody(Message source);
    }
}