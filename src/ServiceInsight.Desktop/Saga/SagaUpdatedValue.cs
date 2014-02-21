using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceBus.Profiler.Desktop.Saga
{
    public class SagaUpdatedValue
    {
        public string Name { get; set; }
        public string NewValue { get; set; }
        public string OldValue { get; set; }

        public string Label
        {
            get
            {
                return string.Format("{0}{1}", Name, IsValueNew ? " (new)" : "");
            }
        }

        public bool IsValueChanged
        {
            get
            {
                return !string.IsNullOrEmpty(OldValue) && NewValue != OldValue;
            }
        }

        public bool IsValueNew
        {
            get
            {
                return string.IsNullOrEmpty(OldValue);
            }
        }

        public bool IsValueNotUpdated
        {
            get
            {
                return !IsValueChanged && !IsValueNew;
            }
        }
    }
}
