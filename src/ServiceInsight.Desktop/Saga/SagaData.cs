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
        public string SagaType { get; set; }
        public Guid SagaId { get; set; }
        public DateTime CompleteTime { get; set; }

    }
}
