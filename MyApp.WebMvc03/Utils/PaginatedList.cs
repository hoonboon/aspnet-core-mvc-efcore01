using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.WebMvc03.Utils
{
    public class PaginatedList<T>: List<T>
    {
        public const int DEFAULT_PAGE_SIZE = 10;

        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(IEnumerable<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

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

        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, int pageIndex, int? pageSize = DEFAULT_PAGE_SIZE)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize.GetValueOrDefault()).Take(pageSize.GetValueOrDefault()).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize.GetValueOrDefault());
        }

    }
}
