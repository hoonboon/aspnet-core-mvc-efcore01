using Microsoft.EntityFrameworkCore;
using MyApp.Common.Public.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.School.Efcore
{
    public class PaginatedListHelper<T>
    {
        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, int pageIndex, int pageSize)
        {
            var totalPages = 1;
            var finalPageIndex = 1;
            IEnumerable<T> items = null;

            var rowCount = await source.CountAsync();
            if (rowCount > 0)
            {
                totalPages = (int)Math.Ceiling(rowCount / (double)pageSize);

                // to ensure that the max pageIndex allowed is always = totalPages
                finalPageIndex = Math.Min(Math.Max(1, pageIndex), totalPages);

                items = await source.Skip((finalPageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            
            return new PaginatedList<T>(items, finalPageIndex, totalPages, pageSize);
        }

    }
}
