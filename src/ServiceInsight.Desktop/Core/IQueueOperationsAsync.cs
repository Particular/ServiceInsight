using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Core
{
    public interface IQueueOperationsAsync : IQueueOperations
    {
        Task<IList<MessageInfo>> GetMessagesAsync(Queue queue);
        Task<int> GetMessageCountAsync(Queue queue);
        Task<IList<Queue>> GetQueuesAsync(string machineName);
        Task<bool> IsMsmqInstalledAsync(string machineName);
    }
}