using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Common.Public.Dtos
{
    public class PaginationDto
    {
        public bool HasPreviousPage { get; set; }

        public bool HasNextPage { get; set; }

        public int PageIndex { get; set; }

        public int TotalPages { get; set; }
        
        public int PageSize { get; set; }
    }
}
