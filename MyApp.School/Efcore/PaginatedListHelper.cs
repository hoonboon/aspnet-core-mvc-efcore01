using Microsoft.EntityFrameworkCore;
using MyApp.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.School.Efcore
{
    public class PaginatedListHelper<T>
    {
        public static async Task<PaginatedListDto<T>> CreateAsync(
            IQueryable<T> source, int pageIndex, int pageSize)
        {
            var rowCount = await source.CountAsync();

            var totalPages = (int)Math.Ceiling(rowCount / (double)pageSize);

            // to ensure that the max pageIndex allowed is always = totalPages
            var finalPageIndex = Math.Min(Math.Max(1, pageIndex), totalPages);

            var items = await source.Skip((finalPageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedListDto<T>(items, finalPageIndex, totalPages);
        }

    }
}
