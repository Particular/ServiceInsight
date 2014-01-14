using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NServiceBus.Profiler.Desktop.Saga
{
    public class SagaData
    {
        public List<SagaUpdate> Changes { get; set; }

        private string sagaType;
        public string SagaType
        { 
            get 
            {
                return ProcessType(sagaType);
            } 
            set 
            {
                sagaType = value;
            } 
        }

        private string ProcessType(string messageType)
        {
            if (string.IsNullOrEmpty(messageType))
                return string.Empty;

            var clazz = messageType.Split(',').First();
            var objectName = clazz.Split('.').Last();

            if (objectName.Contains("+"))
                objectName = objectName.Split('+').Last();

            return objectName;
        }

        public Guid SagaId { get; set; }

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
