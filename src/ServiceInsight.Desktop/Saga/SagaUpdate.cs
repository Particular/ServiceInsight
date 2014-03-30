namespace NServiceBus.Profiler.Desktop.Saga
{
    using Caliburn.PresentationFramework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SagaUpdate : PropertyChangedBase
    {
        public DateTime FinishTime { get; set; }
        public DateTime StartTime { get; set; }
        public SagaStateChangeStatus Status { get; set; }
        public SagaMessage InitiatingMessage { get; set; }
        public List<SagaTimeoutMessage> OutgoingMessages { get; set; }
        public List<SagaUpdatedValue> Values { get; private set; }

        public bool IsFirstNode
        {
            get { return Status == SagaStateChangeStatus.New; }
        }

        public bool IsSagaTimeoutMessage
        {
            get { return InitiatingMessage.IsSagaTimeoutMessage; }
        }

        public List<SagaMessage> NonTimeoutMessages
        {
            get { return OutgoingMessages.Where(m => !m.IsTimeout).Cast<SagaMessage>().ToList(); }
        }

        public List<SagaTimeoutMessage> TimeoutMessages
        {
            get { return OutgoingMessages.Where(m => m.IsTimeout).ToList(); }
        }

        public string StateAfterChange
        {
            get; set;
        }

        public void OnStateAfterChangeChanged()
        {
            Values = JsonPropertiesHelper.ProcessValues(StateAfterChange, s => s.TrimStart('[').TrimEnd(']'))
                             .Select(v => new SagaUpdatedValue
                             {
                                 Name = v.Key,
                                 NewValue = v.Value
                             }).ToList();
        }

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
//            set // this is only for demo deserialization and may be removed soon
//            {
//                if (value == "Saga Completed")
//                {
//                    Status = SagaStateChangeStatus.Completed;
//                }
//                else if (value == "Saga Initiated")
//                {
//                    Status = SagaStateChangeStatus.New;
//                }
//                else if (value == "Saga Updated")
//                {
//                    Status = SagaStateChangeStatus.Updated;
//                }
//            }
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
