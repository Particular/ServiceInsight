using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NServiceBus.Profiler.Desktop.Saga
{
    public class SagaMessage
    {
        public bool IsPublished { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
    }

    public class SagaTimeoutMessage : SagaMessage
    {
        public TimeSpan Timeout { get; set; }

        public string TimeoutFriendly
        {
            get
            {
                return string.Format("{0}{1}{2}", GetFriendly(Timeout.Hours, "h"), GetFriendly(Timeout.Minutes, "m"), GetFriendly(Timeout.Seconds, "s"));
            }
        }

        private string GetFriendly(int time, string text)
        {
            if (time > 0)
            {
                return string.Format("{0}{1}", time, text);
            }
            return string.Empty;
        }
    }
}
