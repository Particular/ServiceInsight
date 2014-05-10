namespace Particular.ServiceInsight.Desktop.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IQueueOperationsAsync : IQueueOperations
    {
        Task<IList<MessageInfo>> GetMessagesAsync(Queue queue);
        Task<int> GetMessageCountAsync(Queue queue);
        Task<IList<Queue>> GetQueuesAsync(string machineName);
        Task<bool> IsMsmqInstalledAsync(string machineName);
        Task<Queue> CreateQueueAsync(Queue queue, bool isTransactional);
    }
}