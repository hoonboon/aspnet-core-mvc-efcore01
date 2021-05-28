using System.Collections.Generic;

namespace MyApp.Common.Public.Dtos
{
    public class BasePaginatedListDto<T>
    {
        public BasePaginatedListDto(PaginatedList<T> listing, ListingFilterSortPageDto filterSortPageValues)
        {
            Listing = listing;
            FilterSortPageValues = filterSortPageValues;
        }

        public PaginatedList<T> Listing { get; set; }

        public ListingFilterSortPageDto FilterSortPageValues { get; set; }
    }
}
