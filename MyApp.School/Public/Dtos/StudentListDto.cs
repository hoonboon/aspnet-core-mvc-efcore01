using MyApp.Common.Public.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyApp.School.Public.Dtos
{
    public class StudentListDto
    {
        public PaginatedListDto<StudentListItem> Listing { get; set; }

        public ListingFilterSortPageDto FilterSortPageValues { get; set; }
    }

    public class StudentListItem
    {
        public int StudentId { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Enrollment Date")]
        public DateTime? EnrollmentDate { get; set; }
    }

    public enum StudentsFilterOptions
    {
        [Display(Name = "Show All")] NoFilter = 0,
        [Display(Name = "Last Name constains")] LastName = 1,
        [Display(Name = "First Name constains")] FirstMidName = 2,
        [Display(Name = "Enrollment Date >=")] EnrollmentDateAfter = 3,
        [Display(Name = "Enrollment Date <=")] EnrollmentDateBefore = 4
    }

    public enum StudentsSortByOptions
    {
        LastName = 0,
        FirstMidName = 1,
        EnrollmentDate = 2
    }
}
