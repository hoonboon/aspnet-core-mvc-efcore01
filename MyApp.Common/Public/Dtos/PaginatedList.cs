using System.Collections.Generic;

namespace MyApp.Common.Public.Dtos
{
    public class PaginatedList<T> : List<T>
    {
        public PaginatedList(
            IEnumerable<T> items, int pageIndex, int totalPages, int pageSize)
        {
            if (items != null)
            {
                AddRange(items);
            }

            PaginationInfo = new PaginationDto
            {
                PageIndex = pageIndex,
                TotalPages = totalPages,
                PageSize = pageSize,
                HasPreviousPage = pageIndex > 1,
                HasNextPage = pageIndex < totalPages
            };
        }

        public PaginationDto PaginationInfo { get; private set; }

    }
}
