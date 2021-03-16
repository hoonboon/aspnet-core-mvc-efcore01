using System.Collections.Generic;

namespace MyApp.Common.Dtos
{
    public class PaginatedListDto<T>: List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedListDto(IEnumerable<T> items, int pageIndex, int totalPages)
        {
            TotalPages = totalPages;
            PageIndex = pageIndex;
            
            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

    }
}
