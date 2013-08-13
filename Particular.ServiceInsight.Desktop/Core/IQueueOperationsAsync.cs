using System.Collections.Generic;
using System.Threading.Tasks;
using Particular.ServiceInsight.Desktop.Core;
using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.Core
{
    public interface IQueueOperationsAsync : IQueueOperations
    {
        Task<IList<MessageInfo>> GetMessagesAsync(Queue queue);
        Task<int> GetMessageCountAsync(Queue queue);
        Task<IList<Queue>> GetQueuesAsync(string machineName);
        Task<bool> IsMsmqInstalled(string machineName);
    }
}