using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NServiceBus.Profiler.Desktop.Saga
{
    public class SagaStep
    {
        public bool IsFirstNode { get; set; }
        public bool IsTimeout { get; set; }

        public SagaMessage StartingMessage { get; set; }
        public IEnumerable<SagaMessage> Messages { get; set; }
        public IEnumerable<SagaTimeoutMessage> TimeoutMessages { get; set; }
        public IEnumerable<SagaUpdatedValue> Values { get; set; }

        public string Label
        {
            get
            {
                return IsFirstNode ? "Saga Initiated" : "Saga Updated";
            }
        }
    }
}
