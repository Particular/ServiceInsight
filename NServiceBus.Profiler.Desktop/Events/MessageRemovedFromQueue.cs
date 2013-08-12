using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.Events
{
    public class MessageRemovedFromQueue
    {
        public MessageInfo Message { get; set; }
    }
}