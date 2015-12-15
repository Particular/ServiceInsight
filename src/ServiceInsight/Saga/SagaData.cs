namespace ServiceInsight.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;
    using Framework;

    public class SagaData : PropertyChangedBase
    {
        public List<SagaUpdate> Changes { get; set; }

        public Guid SagaId { get; set; }

        string sagaType;

        public string SagaType
        {
            get { return TypeHumanizer.ToName(sagaType); }
            set { sagaType = value; }
        }

        public bool IsCompleted
        {
            get
            {
                if (Changes == null)
                    return false;
                return Changes.Any(c => c.Status == SagaStateChangeStatus.Completed);
            }
        }

        public DateTime CompleteTime
        {
            get
            {
                if (Changes == null)
                    return DateTime.MinValue;

                var change = Changes.FirstOrDefault(c => c.Status == SagaStateChangeStatus.Completed);
                if (change == null)
                    return DateTime.MinValue;

                return change.FinishTime;
            }
        }
    }
}