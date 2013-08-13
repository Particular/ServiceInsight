using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.Events
{
    public class MessageBodyLoaded
    {
        public MessageBodyLoaded(MessageBody message)
        {
            Message = message;
        }

        public MessageBody Message { get; private set; }
    }
}