namespace Particular.ServiceInsight.Desktop.Core
{
    using System.Messaging;
    using Models;

    public interface IMapper
    {
        Queue MapQueue(MessageQueue source);
        MessageInfo MapInfo(Message source);
        MessageBody MapBody(Message source);
    }
}