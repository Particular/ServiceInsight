namespace ServiceInsight.Models
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

        public int PageSize { get; set; }

        public string FirstLink { get; set; }

        public string PrevLink { get; set; }

        public string NextLink { get; set; }

        public string LastLink { get; set; }
    }
}