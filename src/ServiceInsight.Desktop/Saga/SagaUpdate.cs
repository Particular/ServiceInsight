using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace NServiceBus.Profiler.Desktop.Saga
{
    public class SagaUpdate
    {
        public bool IsFirstNode
        {
            get
            {
                return Status == SagaStateChangeStatus.New;
            }
        }

        public bool IsSagaTimeoutMessage
        {
            get
            {
                return InitiatingMessage.IsSagaTimeoutMessage;
            }
        }

        public SagaStateChangeStatus Status { get; set; }

        public SagaMessage InitiatingMessage { get; set; }
        public List<SagaTimeoutMessage> OutgoingMessages { get; set; }

        public List<SagaMessage> NonTimeoutMessages
        {
            get
            {
                return OutgoingMessages.Where(m => !m.IsTimeout).Cast<SagaMessage>().ToList();
            }
        }

        public List<SagaTimeoutMessage> TimeoutMessages
        {
            get
            {
                return OutgoingMessages.Where(m => m.IsTimeout).ToList();
            }
        }

        private string stateAfterChange;
        public string StateAfterChange
        {
            get
            {
                return stateAfterChange;
            }
            set
            {
                stateAfterChange = value;
                ProcessValues(stateAfterChange);
            }
        }

        private IList<string> standardKeys = new List<string> { "$type", "Id", "Originator", "OriginalMessageId" };

        private void ProcessValues(string stateAfterChange)
        {
            Values = JsonConvert.DeserializeObject<Dictionary<string, object>>
                (stateAfterChange.TrimStart('[').TrimEnd(']'))
                .Where(m => !standardKeys.Any(s => s == m.Key))
                .Select(n => new SagaUpdatedValue { Name = n.Key, NewValue = n.Value.ToString() })
                .ToList();
        }

        public List<SagaUpdatedValue> Values { get; set; }

        public string Label
        {
            get
            {
                switch (Status)
                {
                    case SagaStateChangeStatus.New:
                        return "Saga Initiated";
                    case SagaStateChangeStatus.Completed:
                    case SagaStateChangeStatus.Updated:
                        return "Saga Updated";
                }

                return string.Empty;
            }
            set // this is only for demo deserialization and may be removed soon
            {
                if (value == "Saga Completed")
                {
                    Status = SagaStateChangeStatus.Completed;
                }
                else if (value == "Saga Initiated")
                {
                    Status = SagaStateChangeStatus.New;
                }
                else if (value == "Saga Updated")
                {
                    Status = SagaStateChangeStatus.Updated;
                }
            }
        }

        public DateTime FinishTime { get; set; }
        public DateTime StartTime { get; set; }
    }

    public enum SagaStateChangeStatus
    {
        Nothing,
        New,
        Updated,
        Completed
    }
}
