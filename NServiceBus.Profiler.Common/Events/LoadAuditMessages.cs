using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Common.Events
{
    public class LoadAuditMessages
    {
        public LoadAuditMessages()
        {
            PageIndex = 1;
        }

        public Endpoint Endpoint { get; set; }

        public int PageIndex { get; set; }

        public string SearchQuery { get; set; }
    }
}