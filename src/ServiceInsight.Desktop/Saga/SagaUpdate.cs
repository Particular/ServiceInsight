using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool IsTimeout { get; set; }

        public SagaStateChangeStatus Status { get; set; }

        public SagaMessage InitiatingMessage { get; set; }
        public List<SagaMessage> OutgoingMessages { get; set; }
        public List<SagaTimeoutMessage> TimeoutMessages { get; set; }
        public List<SagaUpdatedValue> Values { get; set; }

        public string Label
        {
            get
            {
                switch (Status)
                {
                    case SagaStateChangeStatus.Completed:
                        return "Saga Completed";
                    case SagaStateChangeStatus.New:
                        return "Saga Initiated";
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
    }

    public enum SagaStateChangeStatus
    {
        Nothing,
        New,
        Updated,
        Completed
    }
}
