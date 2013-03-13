using System.Collections.Generic;

namespace NServiceBus.Profiler.Common.Models
{
    public class PagedResult<T>
    {
        public PagedResult()
        {
            Result = new List<T>();
        }

        public List<T> Result { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
    }
}