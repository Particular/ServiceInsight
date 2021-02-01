namespace ServiceInsight.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;

    public class SagaUpdate : PropertyChangedBase
    {
        public DateTime FinishTime { get; set; }

        public DateTime StartTime { get; set; }

        public SagaStateChangeStatus Status { get; set; }

        public SagaMessage InitiatingMessage { get; set; }

        public List<SagaTimeoutMessage> OutgoingMessages { get; set; }

        public List<SagaUpdatedValue> Values { get; private set; }

        public bool IsFirstNode => Status == SagaStateChangeStatus.New;

        public bool IsSagaTimeoutMessage => !MissingData && InitiatingMessage.IsSagaTimeoutMessage;

        public List<SagaMessage> NonTimeoutMessages
            => OutgoingMessages?.Where(m => !m.IsTimeout).Cast<SagaMessage>().ToList() ?? new List<SagaMessage>();

        public List<SagaTimeoutMessage> TimeoutMessages
            => OutgoingMessages?.Where(m => m.IsTimeout).ToList() ?? new List<SagaTimeoutMessage>();

        public bool HasNonTimeoutMessages => NonTimeoutMessages.Any();

        public bool HasTimeoutMessages => TimeoutMessages.Any();

        public string StateAfterChange { get; set; }

        public bool MissingData { get; set; }

#pragma warning disable IDE0051 // Remove unused private members
        void OnStateAfterChangeChanged()
#pragma warning restore IDE0051 // Remove unused private members
        {
            UpdateValues();
        }

#pragma warning disable IDE0051 // Remove unused private members
        void OnInitiatingMessageChanged()
#pragma warning restore IDE0051 // Remove unused private members
        {
            UpdateValues();
        }

#pragma warning disable IDE0051 // Remove unused private members
        void OnMissingDataChanged()
#pragma warning restore IDE0051 // Remove unused private members
        {
            UpdateValues();
        }

        void UpdateValues()
        {
            if (string.IsNullOrEmpty(StateAfterChange) || InitiatingMessage == null || MissingData)
            {
                Values = new List<SagaUpdatedValue>();
                return;
            }

            Values = JsonPropertiesHelper.ProcessArray(StateAfterChange)
                                         .Select(v => new SagaUpdatedValue(InitiatingMessage.MessageType, v.Key, v.Value))
                                         .ToList();
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

                    case SagaStateChangeStatus.Nothing:
                    default:
                        return string.Empty;
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