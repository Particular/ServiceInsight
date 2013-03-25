using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Core
{
    public interface IQueueOperationsAsync : IQueueOperations
    {
        /// <summary>
        /// Gets a list of all messages in the queue
        /// </summary>
        Task<IList<MessageInfo>> GetMessagesAsync(Queue queue);
    }
}