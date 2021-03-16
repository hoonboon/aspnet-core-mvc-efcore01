using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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

    public enum PageSizeOptions
    {
        [Display(Name = "10 Rows Per Page")] _10 = 10,
        [Display(Name = "20 Rows Per Page")] _20 = 20,
        [Display(Name = "50 Rows Per Page")] _50 = 50,
        [Display(Name = "100 Rows Per Page")] _100 = 100
    }
}
