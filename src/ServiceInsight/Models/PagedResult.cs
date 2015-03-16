namespace Particular.ServiceInsight.Desktop.Models
{
    using System.Collections.Generic;

    public class PagedResult<T>
    {
        public PagedResult()
        {
            Result = new List<T>();
        }

        public IList<T> Result { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
    }
}