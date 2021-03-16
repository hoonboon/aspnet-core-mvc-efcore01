using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyApp.Common.Dtos
{
    public class ListingFilterSortPageDto
    {
        public static readonly int DEFAULT_PAGE_SIZE = (int)PageSizeOptions._10;

        public int FilterBy { get; set; }

        public string FilterValue { get; set; }

        public int SortBy { get; set; }

        public bool SortAscending { get; set; } = true;

        public int PageIndex { get; set; }

        public int PageSize { get; set; } = ListingFilterSortPageDto.DEFAULT_PAGE_SIZE;

        public string PreviousStateValue { get; set; }

        private string GenerateStateValue()
        {
            return $"{FilterBy},{FilterValue},{PageSize}";
        }

        public void InitDto()
        {
            var newStateValue = this.GenerateStateValue();
            if (this.PreviousStateValue != newStateValue)
            {
                this.PageIndex = 1;
                this.PreviousStateValue = newStateValue;
            }
        }

    }

    public enum PageSizeOptions
    {
        [Display(Name = "10")] _10 = 10,
        [Display(Name = "20")] _20 = 20,
        [Display(Name = "50")] _50 = 50,
        [Display(Name = "100")] _100 = 100
    }
}
